using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ManadaIA.Application.DTOs;
using ManadaIA.Application.ExternalServices.Interfaces;
using ManadaIA.Application.Settings;
using Microsoft.Extensions.Logging;

namespace ManadaIA.Application.ExternalServices
{
    /// <summary>
    /// Implementação da integração com Supabase para autenticação
    /// </summary>
    public sealed class SupabaseAuthService : ISupabaseAuthService
    {
        private const string HttpClientName = "Supabase";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SupabaseSettings _supabaseSettings;
        private readonly ILogger<SupabaseAuthService> _logger;

        public SupabaseAuthService(
            IHttpClientFactory httpClientFactory,
            SupabaseSettings supabaseSettings,
            ILogger<SupabaseAuthService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _supabaseSettings = supabaseSettings ?? throw new ArgumentNullException(nameof(supabaseSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginDto> AuthenticateAsync(UserLoginRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_supabaseSettings.Url) || string.IsNullOrWhiteSpace(_supabaseSettings.AnonKey))
            {
                _logger.LogError("Supabase configuration is missing or empty");
                throw new InvalidOperationException("Supabase configuration is not properly configured");
            }

            var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            var loginUrl = $"{_supabaseSettings.Url.TrimEnd('/')}/auth/v1/token?grant_type=password";

            var requestBody = new
            {
                email = request.User,
                password = request.Password
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            // Ensure header isn't added repeatedly
            if (!httpClient.DefaultRequestHeaders.Contains("apikey"))
            {
                httpClient.DefaultRequestHeaders.Add("apikey", _supabaseSettings.AnonKey);
            }

            try
            {
                _logger.LogInformation("Initiating authentication request to Supabase for user: {Email}", request.User);

                var response = await httpClient.PostAsync(loginUrl, jsonContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    _logger.LogError("Supabase authentication failed with status {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                    throw new InvalidOperationException("Authentication failed. Invalid credentials.");
                }

                var responseContent = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var loginResponse = JsonSerializer.Deserialize<LoginDto>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null)
                {
                    _logger.LogError("Failed to deserialize Supabase authentication response");
                    throw new InvalidOperationException("Failed to process authentication response");
                }

                _logger.LogInformation("User authenticated successfully: {Email}", request.User);
                return loginResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error during authentication");
                throw new InvalidOperationException("Failed to connect to authentication service", ex);
            }
            catch (TaskCanceledException ex) when (ct.IsCancellationRequested == false)
            {
                // Timeout-ish cancellation
                _logger.LogError(ex, "Authentication request timeout");
                throw new InvalidOperationException("Authentication request timeout", ex);
            }
        }
    }
}
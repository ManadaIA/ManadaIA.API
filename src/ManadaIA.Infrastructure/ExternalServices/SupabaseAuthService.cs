using ManadaIA.Application.DTOs;
using ManadaIA.Application.ExternalServices.Interfaces;
using ManadaIA.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ManadaIA.Application.ExternalServices;

/// <summary>
/// Implementação da integração com Supabase para autenticação
/// </summary>
public sealed class SupabaseAuthService(
    IHttpClientFactory httpClientFactory,
    SupabaseSettings supabaseSettings,
    ILogger<SupabaseAuthService> logger) : ISupabaseAuthService
{
    private const string HttpClientName = "Supabase";

    public async Task<LoginDto> AuthenticateAsync(UserLoginRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(supabaseSettings.Url) || string.IsNullOrWhiteSpace(supabaseSettings.AnonKey))
        {
            logger.LogError("Supabase configuration is missing or empty");
            throw new InvalidOperationException("Supabase configuration is not properly configured");
        }

        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        var loginUrl = $"{supabaseSettings.Url.TrimEnd('/')}/auth/v1/token?grant_type=password";

        var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        if (!httpClient.DefaultRequestHeaders.Contains("apikey"))
        {
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseSettings.AnonKey);
        }

        try
        {
            logger.LogInformation("Initiating authentication request to Supabase for user: {Email}", request.Email);

            var response = await httpClient.PostAsync(loginUrl, jsonContent, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                logger.LogError("Supabase authentication failed with status {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                throw new InvalidOperationException("Authentication failed. Invalid credentials.");
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var loginResponse = JsonSerializer.Deserialize<LoginDto>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (loginResponse == null)
            {
                logger.LogError("Failed to deserialize Supabase authentication response");
                throw new InvalidOperationException("Failed to process authentication response");
            }

            logger.LogInformation("User authenticated successfully: {Email}", request.Email);
            return loginResponse;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request error during authentication");
            throw new InvalidOperationException("Failed to connect to authentication service", ex);
        }
        catch (TaskCanceledException ex) when (ct.IsCancellationRequested == false)
        {
            logger.LogError(ex, "Authentication request timeout");
            throw new InvalidOperationException("Authentication request timeout", ex);
        }
    }
}
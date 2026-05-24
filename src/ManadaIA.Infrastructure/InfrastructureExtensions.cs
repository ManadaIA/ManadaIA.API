using ManadaIA.Domain.Interfaces;
using ManadaIA.Infrastructure.Repositories;
using ManadaIA.Infrastructure.Supabase;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManadaIA.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Supabase Settings
        var supabaseSettings = configuration
            .GetSection(SupabaseSettings.SectionName)
            .Get<SupabaseSettings>() ?? throw new InvalidOperationException("Configuração do Supabase não encontrada");

        services.AddSingleton(supabaseSettings);

        // Supabase Client        
        services.AddHttpContextAccessor();
        services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<SupabaseSettings>();

            var httpContextAccessor =
                sp.GetRequiredService<IHttpContextAccessor>();

            var httpContext = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext não encontrado");

            var authHeader = httpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authHeader))
                throw new UnauthorizedAccessException("Token não informado");

            var token = authHeader.Replace("Bearer ", "");

            return SupabaseClientFactory
                .CreateAsync(settings, token)
                .GetAwaiter()
                .GetResult();
        });

        // Repositories
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IReproductiveCycleRepository, ReproductiveCycleRepository>();
        services.AddScoped<IAIPredictionRepository, AIPredictionRepository>();

        return services;
    }
}

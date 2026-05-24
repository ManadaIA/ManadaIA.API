using ManadaIA.Application.ExternalServices;
using ManadaIA.Application.ExternalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ManadaIA.Application.Extensions;

public static class ApplicationExternalExtensions
{
    public static IServiceCollection AddApplicationExternalServices(this IServiceCollection services)
    {
        // External Services
        services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();

        return services;
    }
}
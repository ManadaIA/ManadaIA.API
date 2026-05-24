using ManadaIA.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManadaIA.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IReproductiveCycleService, ReproductiveCycleService>();
        services.AddScoped<IAIPredictionService, AIPredictionService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}

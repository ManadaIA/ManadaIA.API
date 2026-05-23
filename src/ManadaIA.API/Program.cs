using ManadaIA.API.Extensions;
using ManadaIA.API.Middleware;
using ManadaIA.Application;
using ManadaIA.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════
// Configuração de Logging com Serilog
// ══════════════════════════════════════════
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());

// ══════════════════════════════════════════
// Serviços de cada camada
// ══════════════════════════════════════════
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)       // Supabase, Repos, Services
    .AddSupabaseAuth(builder.Configuration);                // JWT Bearer Authentication

// ══════════════════════════════════════════
// Controllers & API Explorer
// ══════════════════════════════════════════
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ══════════════════════════════════════════
// API Versioning
// ══════════════════════════════════════════
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ══════════════════════════════════════════
// Swagger com suporte a JWT
// ══════════════════════════════════════════
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ManadaIA API",
        Version = "v1",
        Description = "API para gestão genética e reprodutiva para criadores de bovinos, ovinos e caprinos do sertão cearense. O produtor cadastra seus animais, registra cada inseminação artificial e recebe análises de Inteligência Artificial que estimam a chance de prenhez, identificam fatores de risco e recomendam os melhores reprodutores e matrizes com base no histórico real do rebanho tudo em português simples, pelo celular ou computador."
    });

    options.SchemaFilter<InfraestructureFilters>();

    var xmlFile = "ManadaIA.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Configurar autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ══════════════════════════════════════════
// Health Checks
// ══════════════════════════════════════════
builder.Services.AddHealthChecks();

// ══════════════════════════════════════════
// CORS
// ══════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ══════════════════════════════════════════
// Pipeline de Middleware
// ══════════════════════════════════════════

// Swagger

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var desc in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"ManadaIA API {desc.GroupName.ToUpperInvariant()}");
    }
    options.RoutePrefix = "documentation";
});

// Middleware de exceções global
app.UseMiddleware<ExceptionMiddleware>();

// Logging de requisições
app.UseSerilogRequestLogging();

// HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Health Check
app.MapHealthChecks("/health");

// ══════════════════════════════════════════
// Iniciar aplicação
// ══════════════════════════════════════════
Log.Information("Iniciando ManadaIA API...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

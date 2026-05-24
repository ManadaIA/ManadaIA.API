using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ManadaIA.API.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções
/// Retorna RFC 7807 ProblemDetails
/// </summary>
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ManadaIA.Domain.Exceptions.NotFoundException notFound => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Recurso não encontrado",
                Detail = notFound.Message,
                Instance = context.Request.Path
            },

            ManadaIA.Domain.Exceptions.ConflictException conflict => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflito",
                Detail = conflict.Message,
                Instance = context.Request.Path
            },

            ManadaIA.Domain.Exceptions.UnauthorizedException unauthorized => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Não autorizado",
                Detail = unauthorized.Message,
                Instance = context.Request.Path
            },

            ArgumentException argument => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Argumento inválido",
                Detail = argument.Message,
                Instance = context.Request.Path
            },

            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.",
                Instance = context.Request.Path
            }
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? 500;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}

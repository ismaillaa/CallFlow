using System.Text.Json;

namespace Api.Middleware;

public class GestionErreursMiddleware(RequestDelegate next, ILogger<GestionErreursMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur non gere");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new { erreur = "Une erreur interne est survenue" });
            await context.Response.WriteAsync(response);
        }
    }
}


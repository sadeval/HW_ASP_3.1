using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class AdminAuthMiddleware
{
    private readonly RequestDelegate _next;

    public AdminAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isAdmin = context.Request.Query.ContainsKey("admin") && context.Request.Query["admin"] == "true";

        if (!isAdmin)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Доступ запрещен");
        }
        else
        {
            await _next(context);
        }
    }
}

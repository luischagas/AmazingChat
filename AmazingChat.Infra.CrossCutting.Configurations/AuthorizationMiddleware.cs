using Microsoft.AspNetCore.Http;

namespace AmazingChat.Infra.CrossCutting.Identity;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var isAuthorized = httpContext.User.Claims.Any(c => true);

        if (!isAuthorized && httpContext.Request.Path.Value != "/Identity/Account/Login" && httpContext.Request.Path.Value != "/Identity/Account/Register")
        {
            httpContext.Response.Redirect("/Identity/Account/Login");
        }

        await _next(httpContext);
    }
}
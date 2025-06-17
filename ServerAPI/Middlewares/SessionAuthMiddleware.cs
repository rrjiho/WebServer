using Microsoft.Extensions.Caching.Distributed;
using ServerAPI.Services;
using System.Security.Claims;

namespace ServerAPI.Middlewares
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
        {
            var path = context.Request.Path.ToString().ToLower();

            if (path.Contains("/api/auth/google-login") || path.Contains("/swagger") || path.Contains("/api/auth/logout"))
            {
                await _next(context);
                return;
            }

            var sessionId = context.Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: No session ID.");
                return;
            }

            var userId = await sessionService.GetUserIdFromSessionAsync(sessionId);
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid session.");
                return;
            }

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Session");
            context.User = new ClaimsPrincipal(identity);

            // 사용자 ID를 HttpContext.Items에도 저장
            context.Items["UserId"] = userId;

            await _next(context);
        }
    }
}

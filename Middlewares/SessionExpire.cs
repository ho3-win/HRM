using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Mhrm.Data;

public class SessionExpireMiddleware
{
    private readonly RequestDelegate _next;

    public SessionExpireMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, HrDbContext db)
    {
        var userId = context.User?.FindFirst("UserId")?.Value;

        if (userId != null)
        {
            var session = await db.UserSessions
                .Where(s => s.UserId == int.Parse(userId) && s.Logout_at == DateTime.MinValue)
                .OrderByDescending(s => s.Login_at)
                .FirstOrDefaultAsync();

           if (session != null && session.Expirees_at < DateTime.Now)
            {
                session.Logout_at = DateTime.Now;

                await db.SaveChangesAsync();

                await context.SignOutAsync("CookieAuth");

                context.Response.Clear();
                context.Response.Redirect("/Account/Login");

                return;
            }
        }

        await _next(context);
    }
}
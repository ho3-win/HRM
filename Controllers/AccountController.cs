using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mhrm.Models;
using Microsoft.AspNetCore.Authorization;
namespace Mhrm.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mhrm.Data;
using Mhrm.Models.ViewModels;


public class AccountController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string username, string password, string role)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            ViewBag.Error = "نام کاربری یا رمز عبور اشتباه است";
            return View();
        }

        var roleName = _context.Roles
            .Where(r => r.Id == user.RoleId)
            .Select(r => r.Name)
            .FirstOrDefault();

        var employee = _context.Employees
            .FirstOrDefault(e => e.Id == user.EmployeeId);

        var fullName = employee != null
            ? employee.FirstName + " " + employee.LastName
            : user.Username;

     
        var permissions = _context.RolePermissions
            .Where(rp => rp.RoleId == user.RoleId)
            .Join(
                _context.Permissions,
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => p.Permission_Key
            )
            .ToList();

        Console.WriteLine("ROLE ID = " + user.RoleId);

        foreach(var p in permissions)
        {
            Console.WriteLine("PERMISSION = " + p);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Role, roleName ?? "Unknown"),
            new Claim("UserId", user.Id.ToString())
        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CookieAuth", principal);

        try
        {
            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

            var session = new UserSession
            {
                UserId = user.Id,
                Session_token = Guid.NewGuid().ToString(),
                Login_at = now,
                Expirees_at = now.AddMinutes(10),
                Ip_address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                User_agent = Request.Headers["User-Agent"].ToString() ?? "Unknown"
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"✅ Session created for user: {user.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Session creation error: {ex.Message}");
        }

        if (user.RoleId == 1 || user.RoleId == 2 || user.RoleId == 3)
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Worker");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (userId != null)
        {
            try
            {
                var session = _context.UserSessions
                    .Where(s => s.UserId == int.Parse(userId) && s.Logout_at == DateTime.MinValue)
                    .OrderByDescending(s => s.Login_at)
                    .FirstOrDefault();

                if (session != null)
                {
                    session.Logout_at = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($" Session updated for user: {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Logout error: {ex.Message}");
            }
        }

        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    private readonly HrDbContext _context;

    public AccountController(HrDbContext context)
    {
        _context = context;
    }
}
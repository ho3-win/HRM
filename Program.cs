using Mhrm.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB CONTEXT 
builder.Services.AddDbContext<HrDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllersWithViews();

//  AUTHENTICATION 
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = false;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

//  AUTHORIZATION 
builder.Services.AddAuthorization();

var app = builder.Build();

//  ERROR HANDLING 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//  STATIC MIDDLEWARE 
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//  AUTH PIPELINE 
app.UseAuthentication();

//  Session Expire Middleware 
app.UseMiddleware<SessionExpireMiddleware>();

app.UseAuthorization();

//  ROUTES 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
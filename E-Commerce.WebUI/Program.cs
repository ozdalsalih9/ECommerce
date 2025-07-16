using E_Commerce.Data;
using E_Commerce.WebUI.Utils;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<DatabaseContext>();

        // Email sender servisi
        builder.Services.AddTransient<ICustomEmailSender, SmtpEmailSender>();

        // Localization servislerini ekle
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        // Authentication ve Authorization yapýlandýrmasý
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
          .AddCookie(options =>
          {
              options.LoginPath = "/Account/SignIn";  // Burayý kesinlikle SignIn olarak ayarla
              options.LogoutPath = "/Account/SignOut";
              options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
              options.SlidingExpiration = true;

              options.Cookie.HttpOnly = true;
              options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS zorlama
              options.Cookie.SameSite = SameSiteMode.Strict;
          });

        // Opsiyonel: Rol bazlý policy tanýmlayabilirsin, ama zorunlu deðil
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Localization ayarlarý
        var supportedCultures = new[]
        {
            new CultureInfo("tr-TR"),
            new CultureInfo("en-US"),
            new CultureInfo("ar-SA")
        };

        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("tr-TR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };

        app.UseRequestLocalization(localizationOptions);
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("Content-Security-Policy",
    "default-src 'self'; " +
    "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com https://unpkg.com https://cdn.jsdelivr.net; " +
    "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://unpkg.com https://use.fontawesome.com https://code.jquery.com https://cdn.jsdelivr.net; " +
    "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
    "img-src 'self' data:; " +
    "connect-src 'self'; " +
    "object-src 'none'; " +
    "base-uri 'self';" +
    "connect-src 'self' http://localhost:59213 ws://localhost:59213 wss://localhost:44377;"

);

            await next();
        });
        app.UseRouting();

        app.UseAuthentication(); // Authentication önce
        app.UseAuthorization();  // Authorization sonra

        // Admin area route — AdminOnly policy uygulamak istersen:
        app.MapAreaControllerRoute(
         name: "admin",
         areaName: "Admin",
         pattern: "Admin/{controller=Main}/{action=Index}/{id?}"
         ).RequireAuthorization("AdminOnly");

        // Normal route
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

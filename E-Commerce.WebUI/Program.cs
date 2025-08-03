using E_Commerce.Data;
using E_Commerce.Service.Abstract;
using E_Commerce.Service.Concrete;
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
        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddTransient<ICustomEmailSender, SmtpEmailSender>();

        // Localization
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
          .AddCookie(options =>
          {
              options.LoginPath = "/Account/SignIn";
              options.LogoutPath = "/Account/SignOut";
              options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
              options.SlidingExpiration = true;
              options.Cookie.HttpOnly = true;
              options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
              options.Cookie.SameSite = SameSiteMode.Strict;
          });

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
            DefaultRequestCulture = new RequestCulture("tr-TR", "tr-TR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };

        // Cookie'den dili oku
        localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());


        // Güvenlik header'larý
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            await next();
        });
        app.UseRequestLocalization(localizationOptions);
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAreaControllerRoute(
         name: "admin",
         areaName: "Admin",
         pattern: "Admin/{controller=Main}/{action=Index}/{id?}"
         ).RequireAuthorization("AdminOnly");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

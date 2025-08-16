using E_Commerce.Data;
using E_Commerce.Service.Abstract;
using E_Commerce.Service.Concrete;
using E_Commerce.WebUI.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // MVC + Localization
        builder.Services.AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("tr-TR"),
                new CultureInfo("en-US")
            };

            options.DefaultRequestCulture = new RequestCulture("tr-TR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            // Cookie -> Query String -> Header
            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
        });

        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddTransient<ICustomEmailSender, SmtpEmailSender>();
        builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

        // 🔐 Cookie Policy (ÇEREZ ONAYI)
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // Non-essential çerezler için kullanıcı onayı zorunlu
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Strict; // istersen Lax yapabilirsin
            options.Secure = CookieSecurePolicy.Always;
            // options.ConsentCookie.IsEssential = true; // framework zaten essential set eder
        });

        // Kimlik doğrulama çerezi
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

              // Kullanıcı isteğiyle (giriş) oluştuğu için "essential" saymak genelde uygundur:
              options.Cookie.IsEssential = true;
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

        // 🧩 Cookie Policy — Authentication/Localization'dan ÖNCE olmalı
        app.UseCookiePolicy();

        // Localization (cookie policy’den sonra)
        app.UseRequestLocalization();

        // Güvenlik başlıkları
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            await next();
        });

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

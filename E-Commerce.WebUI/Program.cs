using E_Commerce.Data;
using E_Commerce.WebUI.Utils;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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

        // Authentication ve Authorization yapýlandýrmasý
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
          options.LoginPath = "/Account/SignIn";  // Burayý kesinlikle SignIn olarak ayarla
          options.LogoutPath = "/Account/SignOut";
          options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
          options.SlidingExpiration = true;
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
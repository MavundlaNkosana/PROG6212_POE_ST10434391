using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization; // Required for CultureInfo
using Microsoft.AspNetCore.Localization; // Required for RequestLocalizationOptions

namespace Contract_Monthly_Claim_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // --- START: Culture Configuration for South African Rand (R) ---
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    // Set the culture to English (South Africa)
                    new CultureInfo("en-ZA")
                };

                // Set the default culture for the application
                options.DefaultRequestCulture = new RequestCulture("en-ZA");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            // --- END: Culture Configuration ---

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // --- ADD THIS LINE: Apply the culture settings to every request ---
            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Claims}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

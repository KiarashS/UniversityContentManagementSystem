using System;
using System.IO;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContentManagement.DataLayer.Context
{
    /// <summary>
    /// Only used by EF Tooling
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();
            services.AddOptions();
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Using `{basePath}` as the BasePath");
            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(basePath)
                                    .AddJsonFile("appsettings.json")
                                    .Build();

            services.AddSingleton<IConfigurationRoot>(provider => configuration);
            services.Configure<SiteSettings>(options => configuration.Bind(options));

            var siteSettings = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<SiteSettings>>();

            services.AddRequiredEfInternalServices(siteSettings.Value); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.SetDbContextOptions(siteSettings.Value);
            optionsBuilder.UseInternalServiceProvider(services.BuildServiceProvider()); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
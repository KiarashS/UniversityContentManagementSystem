using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services.Contracts;
//using ContentManagement.Services.Contracts.Identity;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ContentManagement.IocConfig
{
    public static class IdentityServicesRegistry
    {
        ///// <summary>
        ///// Adds all of the ASP.NET Core Identity related services and configurations at once.
        ///// </summary>
        //public static void AddCustomIdentityServices(this IServiceCollection services)
        //{
        //    var siteSettings = GetSiteSettings(services);
        //    services.AddCustomServices();
        //    services.AddCustomTicketStore(siteSettings);
        //    services.AddDynamicPermissions();
        //    services.AddCustomDataProtection(siteSettings);
        //    services.AddIdentityOptions(siteSettings);
        //}

        ///// <summary>
        ///// Adds all of the ASP.NET Core Identity related initializations at once.
        ///// </summary>
        //public static void UseCustomIdentityServices(this IApplicationBuilder app)
        //{
        //    app.UseAuthentication();
        //    app.callDbInitializer();
        //}

        //private static void callDbInitializer(this IApplicationBuilder app)
        //{
        //    var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
        //    using (var scope = scopeFactory.CreateScope())
        //    {
        //        // var identityDbInitialize = scope.ServiceProvider.GetService<IIdentityDbInitializer>();
        //        var identityDbInitialize = scope.ServiceProvider.GetService<IDbInitializerService>();
        //        identityDbInitialize.Initialize();
        //        identityDbInitialize.SeedData();
        //    }
        //}

        public static SiteSettings GetSiteSettings(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var siteSettingsOptions = provider.GetService<IOptionsSnapshot<SiteSettings>>();
            siteSettingsOptions.CheckArgumentIsNull(nameof(siteSettingsOptions));
            var siteSettings = siteSettingsOptions.Value;
            siteSettings.CheckArgumentIsNull(nameof(siteSettings));
            return siteSettings;
        }
    }
}
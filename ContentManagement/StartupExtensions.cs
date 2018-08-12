using CacheManager.Core;
using ContentManagement.DataLayer.Context;
using ContentManagement.Infrastructure;
using ContentManagement.Services.Seo;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Ganss.XSS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Memory;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Resolvers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ContentManagement
{
    public static class StartupExtensions
    {
        public static void AddInternalServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ICookieValidatorService, CookieValidatorService>();
            services.AddScoped<IDbInitializerService, DbInitializerService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<SeoService>();
            services.AddSingleton<IHtmlSanitizer>(s => new HtmlSanitizer());
            services.AddScoped<IPortalService, PortalService>();
            services.AddScoped<INavbarService, NavbarService>();
            services.AddScoped<ISlideService, SlideService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<IImageLinkService, ImageLinkService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IContentGalleryService, ContentGalleryService>();
            services.AddScoped<IUrlUtilityService, UrlUtilityService>();
        }

        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            // Only needed for custom roles.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
                options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            });

            // Needed for cookie auth.
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.SlidingExpiration = false;
                options.LoginPath = "/login/";
                options.LogoutPath = "/login/logout/";
                //options.AccessDeniedPath = new PathString("/error/index/403/");
                options.Cookie.Name = ".user.portal.cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        var cookieValidatorService = context.HttpContext.RequestServices.GetRequiredService<ICookieValidatorService>();
                        return cookieValidatorService.ValidateAsync(context);
                    }
                };
            });
        }

        public static void UseLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US")
            };
            var supportedUICultures = new List<CultureInfo>
            {
                new CultureInfo("fa-IR"),
                new CultureInfo("en-US")
            };

            var requestLocalizationOptions = new RequestLocalizationOptions();
            requestLocalizationOptions.DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"), new CultureInfo("fa-IR"));
            requestLocalizationOptions.SupportedCultures = supportedCultures;
            requestLocalizationOptions.SupportedUICultures = supportedUICultures;
            requestLocalizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),// { CookieName = ".language.portal.cookie" },
                new FaRequestCultureProvider(),
                //new RouteDataRequestCultureProvider()
            };
            
            app.UseRequestLocalization(requestLocalizationOptions);
        }

        public static void AddInMemoryCacheServiceProvider(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle()
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .Build());
        }

        public static void AddCustomDataProtection(this IServiceCollection services, SiteSettings siteSettings)
        {
            services.AddScoped<IXmlRepository, DataProtectionKeyService>();
            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = scopeFactory.CreateScope())
                    {
                        options.XmlRepository = scope.ServiceProvider.GetService<IXmlRepository>();
                    }
                });
            });
            services
                .AddDataProtection()
                .SetDefaultKeyLifetime(siteSettings.CookieOptions.ExpireTimeSpan)
                .SetApplicationName(siteSettings.CookieOptions.CookieName)
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
        }

        public static void AddCustomImageSharp(this IServiceCollection services)
        {
            services.AddImageSharpCore()
                .SetRequestParser<QueryCollectionRequestParser>()
                .SetBufferManager<PooledBufferManager>()
                .SetCache(provider => new PhysicalFileSystemCache(
                    provider.GetRequiredService<IHostingEnvironment>(),
                    provider.GetRequiredService<IBufferManager>(),
                    provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>())
                {
                    Settings =
                    {
                        [PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder,
                        [PhysicalFileSystemCache.CheckSourceChanged] = "true"
                    }
                })
                .SetCacheHash<CacheHash>()
                .SetAsyncKeyLock<AsyncKeyLock>()
                .AddResolver<PhysicalFileSystemResolver>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>();
        }
    }
}

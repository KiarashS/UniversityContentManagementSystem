using ContentManagement.DataLayer.Context;
using ContentManagement.Infrastructure;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
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
                options.LoginPath = "/api/account/login";
                options.LogoutPath = "/api/account/logout";
                //options.AccessDeniedPath = new PathString("/Home/Forbidden/");
                options.Cookie.Name = ".my.app1.cookie";
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
                new CookieRequestCultureProvider(),
                new FaRequestCultureProvider(),
                //new RouteDataRequestCultureProvider()
            };

            app.UseRequestLocalization(requestLocalizationOptions);
        }
    }
}

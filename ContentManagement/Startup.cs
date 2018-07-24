using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore;
using WebMarkupMin.AspNetCore2;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.ViewModels.Settings;
//using ContentManagement.Common.WebToolkit;
//using ContentManagement.Common.GuardToolkit;
using ContentManagement.Common.PersianToolkit;
using ContentManagement.Services.Contracts;
using ContentManagement.DataLayer.Context;
using ContentManagement.IocConfig;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using Boilerplate.AspNetCore.Filters;
using EFSecondLevelCache.Core;
using DNTCommon.Web.Core;
using Ben.Diagnostics;
using DataTables.AspNet.AspNetCore;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp.Web.DependencyInjection;
using ContentManagement.Infrastructure;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ContentManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(opt => { opt.AddServerHeader = false; opt.Limits.MaxRequestBodySize = 1073741824; /*1GB*/ })
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExceptional(Configuration.GetSection("Exceptional"));//, settings =>
            //{
            //    settings.Store.ApplicationName = "CMS";
            //    //settings.UseExceptionalPageOnThrow = Environment.IsDevelopment();
            //});

            services.AddDNTCommonWeb();
            services.RegisterDataTables();

            services.Configure<SiteSettings>(options => Configuration.Bind(options));
            
            // Adds all of the ASP.NET Core Identity related services and configurations at once.
            // services.AddCustomIdentityServices();

            services.AddHttpContextAccessor();
            services.AddInternalServices();

            var siteSettings = services.GetSiteSettings();
            services.AddRequiredEfInternalServices(siteSettings); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            services.AddDbContextPool<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.SetDbContextOptions(siteSettings);
                optionsBuilder.UseInternalServiceProvider(serviceProvider); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            });

            services.AddAuthenticationServices();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder
            //            .AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .AllowCredentials());
            //});

            services.AddResponseCaching();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var lowercaseUrls = true;
            var appendTrailingSlash = true;
            services.AddRouting(options =>
            {
                options.LowercaseUrls = lowercaseUrls;
                options.AppendTrailingSlash = appendTrailingSlash;
            });

            services.AddMvc(options =>
            {
                options.UseCustomStringModelBinder();
                options.AllowEmptyInputInBodyModelBinding = true;
                options.Filters.Add(new RequireWwwAttribute
                {
                    IgnoreLocalhost = false,
                    Permanent = true
                });
                options.Filters.Add(new RedirectToCanonicalUrlAttribute(appendTrailingSlash, lowercaseUrls));
                options.Filters.Add(new NoLowercaseQueryStringAttribute());
                options.Filters.Add(typeof(SetSeoMetaValuesAttribute));
                //options.Filters.Add(new RequireHttpsAttribute() { Permanent = true });
                // options.Filters.Add(new NoBrowserCacheAttribute());
            })
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.DisablePoweredByHttpHeaders = true;
                options.DisableCompression = true;
            })
            .AddHtmlMinification(options =>
            {
                options.MinificationSettings.RemoveRedundantAttributes = true;
                options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                options.MinificationSettings.MinifyEmbeddedCssCode = false;
                options.MinificationSettings.MinifyEmbeddedJsCode = false;
                options.MinificationSettings.RemoveOptionalEndTags = false;
                options.MinificationSettings.MinifyInlineCssCode = true;
                options.MinificationSettings.MinifyInlineJsCode = true;
                options.MinificationSettings.AttributeQuotesRemovalMode = WebMarkupMin.Core.HtmlAttributeQuotesRemovalMode.KeepQuotes;
            });

            services.AddRazorViewRenderer();
            //services.AddDNTCaptcha();
            services.AddMvcActionsDiscoveryService();
            services.AddProtectionProviderService();
            ContentManagement.Common.GuardToolkit.RijndaelProviderServiceExtensions.AddRijndaelProviderService(services);
            services.AddCloudscribePagination();

            services.AddEFSecondLevelCache();
            services.AddInMemoryCacheServiceProvider();

            services.AddCustomImageSharp();
            services.AddCloudscribePagination();

            //services.AddHttpsRedirection(options => options.HttpsPort = 8930);
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(100);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Injecting the Owasp recommended HTTP Headers for increased security
            // app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration());
            app.UseCustomHeader("server", "KIA");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseResponseCaching();
                //app.UseHsts();
                app.UseExceptionHandler("/error/index/500");
                app.UseStatusCodePagesWithReExecute("/error/index/{0}");
            }

            //app.UseHttpsRedirection();
            //app.UseBlockingDetection();
            app.UseLocalization();
            app.UseAuthentication();

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }
            
            app.UseEFSecondLevelCache();

            app.UseImageSharp();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
            //    RequestPath = new PathString("/.well-known"),
            //    ServeUnknownFileTypes = true // serve extensionless files
            //});

            // Serve wwwroot as root
            //app.UseFileServer();

            //app.UseFileServer(new FileServerOptions
            //{
            //    // Set root of file server
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "bower_components")),
            //    // Only react to requests that match this path
            //    RequestPath = "/bower_components",
            //    // Don't expose file system
            //    EnableDirectoryBrowsing = false
            //});

            app.UseWebMarkupMin();

            // Adds all of the ASP.NET Core Identity related initializations at once.
            // app.UseCustomIdentityServices();

            // app.UseNoBrowserCache();

            app.UseExceptional();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "exceptionsRoute",
                    template: "exceptions/{*assets}",
                    defaults: new { controller = "Home", action = "Exceptions" }
                );

                routes.MapRoute(
                    name: "pageRoute",
                    template: "Page/{slug}",
                    defaults: new { controller = "Page", action = "Index" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}/{title?}"
                );

                //routes.MapRoute(
                //    name: "default",
                //    template: "{*catchall}",
                //    defaults: new { controller = "Home", action = "RedirectToDefaultLanguage" }
                //);
            });
        }
    }
}

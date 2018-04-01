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
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.Common.PersianToolkit;
using ContentManagement.Services.Contracts;
using ContentManagement.DataLayer.Context;
using ContentManagement.IocConfig;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor;

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
                .UseKestrel(opt => { opt.AddServerHeader = false; })
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
            //    settings.Store.Type = "JSON";
            //    settings.Store.Path = "Errors";
            //    settings.Store.Size = 500;
            //    //settings.UseExceptionalPageOnThrow = Environment.IsDevelopment();
            //});

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

            services.AddMvc(options =>
            {
                options.UseCustomStringModelBinder();
                options.AllowEmptyInputInBodyModelBinding = true;
                // options.Filters.Add(new NoBrowserCacheAttribute());
            })
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMiniProfiler(options =>
            {
                options.ResultsAuthorize = request => Environment.IsDevelopment();
                options.ResultsListAuthorize = request => Environment.IsDevelopment();
            }).AddEntityFramework();

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
                options.MinificationSettings.RemoveOptionalEndTags = false;
            });

            services.AddRazorViewRenderer();
            //services.AddDNTCaptcha();
            services.AddMvcActionsDiscoveryService();
            services.AddProtectionProviderService();
            services.AddCloudscribePagination();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Injecting the Owasp recommended HTTP Headers for increased security
            // app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseResponseCaching();
                app.UseExceptionHandler("/error/index/500");
                app.UseStatusCodePagesWithReExecute("/error/index/{0}");
            }

            app.UseMiniProfiler();

            app.UseExceptional();

            app.UseLocalization();

            app.UseAuthentication();

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
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

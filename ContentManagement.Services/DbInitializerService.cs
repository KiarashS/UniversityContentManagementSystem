using System;
using System.Linq;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.DataLayer.Context;
using ContentManagement.Entities;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ContentManagement.Services
{
    public class DbInitializerService : IDbInitializerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISecurityService _securityService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public DbInitializerService(
            IServiceScopeFactory scopeFactory,
            ISecurityService securityService,
            IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(_siteSettings));
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    // Add default roles
                    var adminRole = new Role { Name = CustomRoles.Admin };
                    var userRole = new Role { Name = CustomRoles.User };
                    if (!context.Set<Role>().Any())
                    {
                        context.Add(adminRole);
                        context.Add(userRole);
                        context.SaveChanges();
                    }
                    
                    // Add Admin user
                    var siteSettings = _siteSettings.Value;
                    if (!context.Set<User>().Any())
                    {
                        var adminUser = new User
                        {
                            Username = siteSettings.AdminUserSeed.Username,
                            DisplayName = siteSettings.AdminUserSeed.DisplayName,
                            Email = siteSettings.AdminUserSeed.Email,
                            IsActive = true,
                            LastLoggedIn = null,
                            LastLoggedIp = null,
                            Password = _securityService.GetSha256Hash(siteSettings.AdminUserSeed.Password),
                            SerialNumber = Guid.NewGuid().ToString("N")
                        };
                        context.Add(adminUser);
                        context.SaveChanges();

                        var roles = siteSettings.AdminUserSeed.RolesName;
                        if (roles.Any(r => string.Equals(r, CustomRoles.Admin, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            context.Add(new UserRole { Role = adminRole, User = adminUser });
                        }

                        if (roles.Any(r => string.Equals(r, CustomRoles.User, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            context.Add(new UserRole { Role = userRole, User = adminUser });
                        }
                        context.SaveChanges();
                    }
                }
            }
        }        
    }
}
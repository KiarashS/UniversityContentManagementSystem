using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.DataLayer.Context;
using ContentManagement.Entities;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;
        private readonly ISecurityService _securityService;

        public UsersService(IUnitOfWork uow, ISecurityService securityService)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _users = _uow.Set<User>();

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));
        }

        public async Task AddOrUpdateUserAsync(UserViewModel user)
        {
            if (user.Id == 0) // Add
            {
                var newUser = new User
                {
                    DisplayName = user.DisplayName.Trim(),
                    Username = user.Username.Trim(),
                    Email = user.Email.Trim(),
                    Password = user.Password,
                    IsActive = user.IsActive,
                    PortalId = user.PortalId,
                    SerialNumber = Guid.NewGuid().ToString("N")
                };

                newUser.UserRoles.Add(new UserRole { RoleId = 2 }); // RoleId 2 is User
                if (user.IsAdmin)
                {
                    newUser.UserRoles.Add(new UserRole { RoleId = 1 }); // RoleId 1 is Admin
                }

                _users.Add(newUser);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentUser = await FindUserIncludeRolesAsync(user.Id).ConfigureAwait(false);
            var isAdmin = await IsAdminAsync(user.Id).ConfigureAwait(false);

            currentUser.DisplayName = user.DisplayName.Trim();
            currentUser.Username = user.Username.Trim();
            currentUser.Email = user.Email.Trim();
            currentUser.IsActive = user.IsActive;
            currentUser.PortalId = user.PortalId;
            currentUser.SerialNumber = Guid.NewGuid().ToString("N");

            //if (!string.IsNullOrEmpty(user.Password))
            //{
            //    currentUser.Password = user.Password;
            //}

            if (!user.IsAdmin && isAdmin)
            {
                var userRole = currentUser.UserRoles.Single(x => x.RoleId == 1 && x.UserId == user.Id); // RoleId 1 is Admin
                currentUser.UserRoles.Remove(userRole);
            }
            else if (user.IsAdmin && !isAdmin)
            {
                currentUser.UserRoles.Add(new UserRole { RoleId = 1 }); // RoleId 1 is Admin
            }

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<User> FindUserAsync(long userId)
        {
            return _users.FindAsync(userId);
        }

        public Task<User> FindUserIncludeRolesAsync(long userId)
        {
            return _users.Where(x => x.Id == userId).Include(x => x.UserRoles).SingleOrDefaultAsync();
        }

        public Task<User> FindUserAsync(string username, string password)
        {
            var passwordHash = _securityService.GetSha256Hash(password);
            return _users.FirstOrDefaultAsync(x => x.Username == username && x.Password == passwordHash);
        }

        public Task<User> FindUserByEmailAsync(string email, string password)
        {
            var passwordHash = _securityService.GetSha256Hash(password);
            return _users.FirstOrDefaultAsync(x => x.Email == email && x.Password == passwordHash);
        }

        public async Task<IList<UserViewModel>> GetPagedUsersAsync(int? portalId, string searchTerm = null, int start = 0, int length = 20)
        {
            var query = _users.AsQueryable();

            if (portalId.HasValue)
            {
                query = query.Where(x => x.PortalId == portalId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Username.Contains(searchTerm) || x.Email.Contains(searchTerm) || x.DisplayName.Contains(searchTerm));
            }

            var users = await query
                                    .OrderByDescending(x => x.Id)
                                    .Skip(start)
                                    .Take(length)
                                    .Include(x => x.UserRoles)
                                    .Select(x => new { x.Id, x.Username, x.Email, x.DisplayName, x.LastIp, x.LastLogIn, x.IsActive, x.PortalId, x.UserRoles })
                                    .Cacheable()
                                    .ToListAsync();

            return users.Select(x => new UserViewModel { Id = x.Id, Username = x.Username, Email = x.Email, PortalId = x.PortalId, LastIp = x.LastIp, LastLogIn = x.LastLogIn, IsActive = x.IsActive, IsAdmin = x.UserRoles.Count == 2, DisplayName = x.DisplayName }).ToList();
        }

        public async Task<string> GetSerialNumberAsync(long userId)
        {
            var user = await FindUserAsync(userId).ConfigureAwait(false);
            return user.SerialNumber;
        }

        public async Task UpdateUserIpAsync(long userId, string ipAddress)
        {
            var user = await FindUserAsync(userId).ConfigureAwait(false);

            user.LastIp = ipAddress;
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateUserLastActivityDateAsync(long userId)
        {
            var user = await FindUserAsync(userId).ConfigureAwait(false);
            if (user.LastLogIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLogIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }
            user.LastLogIn = DateTimeOffset.UtcNow;
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateUserPasswordAsync(long userId, string password)
        {
            var user = await FindUserAsync(userId).ConfigureAwait(false);

            user.Password = password;
            user.SerialNumber = Guid.NewGuid().ToString("N");
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<long> UsersCountAsync()
        {
            var count = await _users.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<long> UsersPagedCountAsync(int? portalId, string searchTerm = null)
        {
            var query = _users.AsQueryable();

            if (portalId.HasValue)
            {
                query = query.Where(x => x.PortalId == portalId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Username.Contains(searchTerm) || x.Email.Contains(searchTerm) || x.DisplayName.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            var isExist = await _users.Where(u => u.Email == email.Trim()).Cacheable().AnyAsync().ConfigureAwait(false);
            return isExist;
        }

        public async Task<bool> ValidateUsernameAsync(string username)
        {
            var isExist = await _users.Where(u => u.Username == username.Trim()).Cacheable().AnyAsync().ConfigureAwait(false);
            return isExist;
        }

        public async Task DeleteUserAsync(long id)
        {
            var user = new User { Id = id };

            _uow.Entry(user).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsAdminAsync(long id)
        {
            var isAdmin = await _users.AnyAsync(x => x.Id == id && x.UserRoles.Count == 2);
            return isAdmin;
        }

        public async Task<int?> GetPortalIdAsync(string email)
        {
            var portalId = await _users.Where(x => x.Email == email).Select(x => x.PortalId).SingleAsync();
            return portalId;
        }

        public async Task ResetPasswordAsync(long id, string newPassword)
        {
            var currentUser = await FindUserIncludeRolesAsync(id).ConfigureAwait(false);
            currentUser.Password = newPassword;
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

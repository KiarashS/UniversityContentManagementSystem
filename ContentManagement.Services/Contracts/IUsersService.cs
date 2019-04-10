using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IUsersService
    {
        Task<string> GetSerialNumberAsync(long userId);
        Task<User> FindUserAsync(string username, string password);
        Task<User> FindUserByEmailAsync(string email, string password);
        Task<User> FindUserAsync(long userId);
        Task UpdateUserLastActivityDateAsync(long userId);
        Task UpdateUserIpAsync(long userId, string ipAddress);
        Task UpdateUserPasswordAsync(long userId, string password);
        Task<IList<UserViewModel>> GetPagedUsersAsync(int? portalId, string searchTerm = null, int start = 0, int length = 20);
        Task<long> UsersCountAsync();
        Task<long> UsersPagedCountAsync(int? portalId, string searchTerm = null);
        Task AddOrUpdateUserAsync(UserViewModel user);
        Task<bool> ValidateUsernameAsync(string username);
        Task<bool> ValidateEmailAsync(string email);
        Task DeleteUserAsync(long id);
        Task<bool> IsAdminAsync(long id);
        Task<User> FindUserIncludeRolesAsync(long userId);
        Task<int?> GetPortalIdAsync(string email);
        Task ResetPasswordAsync(long id, string newPassword);
    }
}

using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IRolesService
    {
        Task<List<Role>> FindUserRolesAsync(long userId);
        Task<bool> IsUserInRole(long userId, string roleName);
        Task<List<User>> FindUsersInRoleAsync(string roleName);
    }
}

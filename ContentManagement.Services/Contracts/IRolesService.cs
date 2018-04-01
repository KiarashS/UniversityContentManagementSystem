using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IRolesService
    {
        Task<List<Role>> FindUserRolesAsync(int userId);
        Task<bool> IsUserInRole(int userId, string roleName);
        Task<List<User>> FindUsersInRoleAsync(string roleName);
    }
}

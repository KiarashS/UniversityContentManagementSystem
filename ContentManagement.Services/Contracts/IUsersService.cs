using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IUsersService
    {
        Task<string> GetSerialNumberAsync(int userId);
        Task<User> FindUserAsync(string username, string password);
        Task<User> FindUserAsync(int userId);
        Task UpdateUserLastActivityDateAsync(int userId);
    }
}

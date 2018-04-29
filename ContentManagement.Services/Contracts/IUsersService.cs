﻿using ContentManagement.Entities;
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
    }
}

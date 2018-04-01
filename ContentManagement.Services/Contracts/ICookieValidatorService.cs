using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface ICookieValidatorService
    {
        Task ValidateAsync(CookieValidatePrincipalContext context);
    }
}

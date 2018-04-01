using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContentManagement.Common;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.Entities;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ContentManagement.Services
{
    public class CookieValidatorService : ICookieValidatorService
    {
        private readonly IUsersService _usersService;
        public CookieValidatorService(IUsersService usersService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));
        }

        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                // this is not our issued cookie
                await handleUnauthorizedRequest(context);
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                // this is not our issued cookie
                await handleUnauthorizedRequest(context);
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                // this is not our issued cookie
                await handleUnauthorizedRequest(context);
                return;
            }

            var user = await _usersService.FindUserAsync(userId).ConfigureAwait(false);
            if (user == null || user.SerialNumber != serialNumberClaim.Value || !user.IsActive)
            {
                // user has changed his/her password/roles/stat/IsActive
                await handleUnauthorizedRequest(context);
            }

            await _usersService.UpdateUserLastActivityDateAsync(userId).ConfigureAwait(false);
        }

        private Task handleUnauthorizedRequest(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
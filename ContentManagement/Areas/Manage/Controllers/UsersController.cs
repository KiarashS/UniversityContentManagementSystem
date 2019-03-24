﻿using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using ContentManagement.Common.WebToolkit;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class UsersController : Controller
    {
        private readonly IUsersService _userService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;
        private readonly ISecurityService _securityService;

        public UsersController(IUsersService userService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env, ISecurityService securityService)
        {
            _userService = userService;
            _userService.CheckArgumentIsNull(nameof(userService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(securityService));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int? portalId, Entities.Language language)
        {
            var users = await _userService.GetPagedUsersAsync(portalId, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var usersCount = await _userService.UsersCountAsync();
            var usersPagedCount = await _userService.UsersPagedCountAsync(portalId, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)usersCount, (int)usersPagedCount, users);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            return View(new UserViewModel { IsActive = true });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(UserViewModel user)
        {
            user.Password = _securityService.GetSha256Hash(user.Password);
            await _userService.AddOrUpdateUserAsync(user).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "users", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var user = await _userService.FindUserAsync(id);

            if (user == null)
            {
                return RedirectToAction("index", "users", "manage");
            }

            var isAdmin = await _userService.IsAdminAsync(id);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = isAdmin,
                IsActive = user.IsActive,
                PortalId = user.PortalId,
            };

            return View(userViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(UserViewModel user)
        {
            await _userService.AddOrUpdateUserAsync(user).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "users", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            if (_siteSettings.Value.AdminId == id)
            {
                return Content("ادمین اصلی را نمی توانید حذف نمائید.", "text/html", System.Text.Encoding.UTF8);
            }

            await _userService.DeleteUserAsync(id);

            return Content("کاربر با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckUsername(string username, string initialUsername)
        {
            if (!string.IsNullOrEmpty(initialUsername) && username.Trim() == initialUsername.Trim())
            {
                return Json(true);
            }

            return Json(!await _userService.ValidateUsernameAsync(username));
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckEmail(string email, string initialEmail)
        {
            if (!string.IsNullOrEmpty(initialEmail) && email.Trim() == initialEmail.Trim())
            {
                return Json(true);
            }

            return Json(!await _userService.ValidateEmailAsync(email));
        }
    }
}
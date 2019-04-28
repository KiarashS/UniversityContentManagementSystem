using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using System;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class ChangePasswordController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ISecurityService _securityService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly ISet<string> _passwordsBanList;

        public ChangePasswordController(
            IUsersService usersService,
            ISecurityService securityService,
            IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(securityService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _passwordsBanList = new HashSet<string>(_siteSettings.Value.PasswordsBanList, StringComparer.OrdinalIgnoreCase);

            if (!_passwordsBanList.Any())
            {
                throw new InvalidOperationException("Please fill the passwords ban list in the appsettings.json file.");
            }
        }

        public virtual IActionResult Index()
        {
            ViewBag.Title = "تغییر کلمه عبور";
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Index(ChangePasswordModel pass)
        {
            ViewBag.Title = "تغییر کلمه عبور";

            if(string.IsNullOrEmpty(pass.OldPassword))
            {
                ModelState.AddModelError("", "لطفاً کلمه عبور فعلی را وارد نمایید.");
            }

            if (string.IsNullOrEmpty(pass.NewPassword))
            {
                ModelState.AddModelError("", "لطفاً کلمه عبور جدید را وارد نمایید.");
            }

            if (string.IsNullOrEmpty(pass.NewPasswordConfirm))
            {
                ModelState.AddModelError("", "لطفاً تکرار کلمه عبور جدید را وارد نمایید.");
            }

            if (!string.Equals(pass.NewPassword, pass.NewPasswordConfirm))
            {
                ModelState.AddModelError("", "تکرار کلمه عبور جدید با کلمه عبور جدید یکسان نیست!");
            }

            if (!ModelState.IsValid)
            {
                return View(pass);
            }

            var userId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _usersService.FindUserAsync(userId).ConfigureAwait(false);

            if (user == null || !user.IsActive)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ModelState.AddModelError("", "چنین کاربری موجود نیست و یا غیرفعال می باشد.");
                return Redirect("/");
            }

            if(user.Password != _securityService.GetSha256Hash(pass.OldPassword))
            {
                ModelState.AddModelError("", "کلمه عبور فعلی اشتباه می باشد.");
            }

            if (pass.NewPassword.Length < 8)
            {
                ModelState.AddModelError("", "طول کلمه عبور جدید باید حداقل 8 کاراکتر باشد.");
            }

            if (!ModelState.IsValid)
            {
                return View(pass);
            }

            await _usersService.UpdateUserPasswordAsync(userId, _securityService.GetSha256Hash(pass.NewPassword)).ConfigureAwait(false);

            ViewBag.IsOk = true;

            return View();
        }

        private static bool areAllCharsEuqal(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            data = data.ToLowerInvariant();
            var firstElement = data.ElementAt(0);
            var euqalCharsLen = data.ToCharArray().Count(x => x == firstElement);
            if (euqalCharsLen == data.Length) return true;
            return false;
        }

        [HttpPost]
        public virtual IActionResult IsSafePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return Json(false);
            if (newPassword.Length < 8) return Json(false);
            if (_passwordsBanList.Contains(newPassword.ToLowerInvariant())) return Json(false);
            if (areAllCharsEuqal(newPassword)) return Json(false);

            return Json(true);
        }
    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "لطفاً کلمه عبور را وارد نمایید.")]
        [MinLength(8, ErrorMessage = "طول کلمه عبور حداقل {1} کاراکتر می باشد.")]
        [Remote("issafepassword", "changepassword", "manage", HttpMethod = "post", ErrorMessage = "کلمه عبور وارد شده امن نمی باشد! کلمه عبور دیگری وارد نمائید.")]
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}

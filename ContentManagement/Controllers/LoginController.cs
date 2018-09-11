using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ContentManagement.Entities;
using System;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.Services;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;

namespace ContentManagement.Controllers
{
    public partial class LoginController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IRolesService _rolesService;
        private readonly IActivityLogService _logs;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public LoginController(
            IUsersService usersService,
            IRolesService rolesService,
            IActivityLogService logs,
            IRequestService requestService,
            IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _rolesService = rolesService;
            _rolesService.CheckArgumentIsNull(nameof(rolesService));

            _logs = logs;
            _logs.CheckArgumentIsNull(nameof(logs));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async virtual Task<IActionResult> Index(string returnUrl = null)
        {
            await _logs.CreateActivityLogAsync(new ActivityLogViewModel
            {
                ActionBy = User.Identity.IsAuthenticated ? User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value : "--کاربر مهمان",
                ActionType = "login",
                Portal = _requestService.CurrentPortal(),
                Language = _requestService.CurrentLanguage().Language.ToString(),
                Message = $"ورود به صفحه لاگین",
                SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Url = Request.GetDisplayUrl()
            });

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Index(LoginModel loginUser, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (loginUser == null)
            {
                ModelState.AddModelError("", "لطفاً پست الکترونیک و کلمه عبور را وارد نمائید.");
                await _logs.CreateActivityLogAsync(new ActivityLogViewModel
                {
                    ActionBy = "--کاربر مهمان",
                    ActionType = "login",
                    Portal = _requestService.CurrentPortal(),
                    Language = _requestService.CurrentLanguage().Language.ToString(),
                    Message = $"وارد نکردن ایمیل و رمز عبور",
                    SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Url = Request.GetDisplayUrl()
                });
                return View(loginUser);
            }

            if (string.IsNullOrEmpty(loginUser.Email))
            {
                ModelState.AddModelError(loginUser.Email, "لطفاً پست الکترونیک را وارد نمائید.");
                await _logs.CreateActivityLogAsync(new ActivityLogViewModel
                {
                    ActionBy = "--کاربر مهمان",
                    ActionType = "login",
                    Portal = _requestService.CurrentPortal(),
                    Language = _requestService.CurrentLanguage().Language.ToString(),
                    Message = $"وارد نکردن ایمیل",
                    SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Url = Request.GetDisplayUrl()
                });
                return View(loginUser);
            }

            if (string.IsNullOrEmpty(loginUser.Password))
            {
                ModelState.AddModelError(loginUser.Password, "لطفاً کلمه عبور را وارد نمائید.");
                await _logs.CreateActivityLogAsync(new ActivityLogViewModel
                {
                    ActionBy = "--کاربر مهمان",
                    ActionType = "login",
                    Portal = _requestService.CurrentPortal(),
                    Language = _requestService.CurrentLanguage().Language.ToString(),
                    Message = $"وارد نکردن رمز عبور، ایمیل وارد شده: {loginUser.Email}",
                    SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Url = Request.GetDisplayUrl()
                });
                return View(loginUser);
            }

            var user = await _usersService.FindUserByEmailAsync(loginUser.Email, loginUser.Password).ConfigureAwait(false);
            if (user == null || !user.IsActive)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ModelState.AddModelError("", "چنین کاربری موجود نیست و یا غیرفعال می باشد.");
                await _logs.CreateActivityLogAsync(new ActivityLogViewModel
                {
                    ActionBy = "--کاربر مهمان",
                    ActionType = "login",
                    Portal = _requestService.CurrentPortal(),
                    Language = _requestService.CurrentLanguage().Language.ToString(),
                    Message = $"چنین کاربری موجود نیست و یا غیرفعال می باشد، ایمیل وارد شده: {loginUser.Email}",
                    SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Url = Request.GetDisplayUrl()
                });
                return View(loginUser);
            }

            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "پست الکترونیک و یا کلمه عبور اشتباه می باشند.");
                await _logs.CreateActivityLogAsync(new ActivityLogViewModel
                {
                    ActionBy = "--کاربر مهمان",
                    ActionType = "login",
                    Portal = _requestService.CurrentPortal(),
                    Language = _requestService.CurrentLanguage().Language.ToString(),
                    Message = $"پست الکترونیک و یا کلمه عبور اشتباه می باشند، ایمیل وارد شده: {loginUser.Email}",
                    SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Url = Request.GetDisplayUrl()
                });
                return View(loginUser);
            }

            var loginCookieExpirationDays = _siteSettings.Value.LoginCookieExpirationDays;
            var cookieClaims = await createCookieClaimsAsync(user).ConfigureAwait(false);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                cookieClaims,
                new AuthenticationProperties
                {
                    IsPersistent = false, // "Remember Me"
                    IssuedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(loginCookieExpirationDays)
                });

            var userIpAddress = HttpContext.Connection.RemoteIpAddress;
            await _usersService.UpdateUserLastActivityDateAsync(user.Id).ConfigureAwait(false);
            await _usersService.UpdateUserIpAsync(user.Id, userIpAddress.ToString());

            await _logs.CreateActivityLogAsync(new ActivityLogViewModel
            {
                ActionBy = "--کاربر مهمان",
                ActionType = "login",
                Portal = _requestService.CurrentPortal(),
                Language = _requestService.CurrentLanguage().Language.ToString(),
                Message = $"ورود موفقیت آمیز. ایمیل: {loginUser.Email}",
                SourceAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                ActionLevel = ActionLevel.High,
                Url = Request.GetDisplayUrl()
            });

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(url: returnUrl);
            }

            return RedirectToAction("index", "home");
        }

        private async Task<ClaimsPrincipal> createCookieClaimsAsync(User user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim("DisplayName", user.DisplayName));

            // to invalidate the cookie
            identity.AddClaim(new Claim(ClaimTypes.SerialNumber, user.SerialNumber));

            // custom data
            identity.AddClaim(new Claim(ClaimTypes.UserData, user.Id.ToString()));

            // add roles
            var roles = await _rolesService.FindUserRolesAsync(user.Id).ConfigureAwait(false);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }

            return new ClaimsPrincipal(identity);
        }

        public async virtual Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        //[HttpGet("[action]"), HttpPost("[action]")]
        //public virtual bool IsAuthenthenticated()
        //{
        //    return User.Identity.IsAuthenticated;
        //}

        //[HttpGet("[action]"), HttpPost("[action]")]
        //public virtual IActionResult GetUserInfo()
        //{
        //    var claimsIdentity = User.Identity as ClaimsIdentity;
        //    return Json(new { Username = claimsIdentity.Name });
        //}
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

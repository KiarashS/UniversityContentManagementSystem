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

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class ChangePasswordController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ISecurityService _securityService;

        public ChangePasswordController(
            IUsersService usersService,
            ISecurityService securityService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(securityService));
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
    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}

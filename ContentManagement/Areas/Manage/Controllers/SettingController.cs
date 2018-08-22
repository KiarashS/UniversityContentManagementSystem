using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.ViewModels.Areas.Manage;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class SettingController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public SettingController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(environment));
        }

        public virtual IActionResult Logo()
        {
            ViewBag.Title = "مدیریت لوگوها";
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async virtual Task<IActionResult> Logo(LogoViewModel logos)
        {
            if (!ModelState.IsValid)
            {
                return View(logos);
            }

            if (logos.HeaderLogo.IsImageFile())
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "statics", "headerlogo.png");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logos.HeaderLogo.CopyToAsync(fileStream);
                }
            }

            if (logos.Logo.IsImageFile())
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "statics", "logo.png");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logos.Logo.CopyToAsync(fileStream);
                }
            }

            if (logos.FooterLogo.IsImageFile())
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "statics", "footerlogo.png");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logos.FooterLogo.CopyToAsync(fileStream);
                }
            }

            if (logos.Favicon.IsImageFile())
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "favicon.png");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logos.Favicon.CopyToAsync(fileStream);
                }
            }

            return View(logos);
        }
    }
}

using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using DataTables.AspNet.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContentManagement.ViewModels.Settings;
using Microsoft.Extensions.Options;
using DataTables.AspNet.AspNetCore;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class NavbarController : Controller
    {
        private readonly INavbarService _navbarService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        public NavbarController(INavbarService navbarService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _navbarService = navbarService;
            _navbarService.CheckArgumentIsNull(nameof(navbarService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language)
        {
            var navbars = await _navbarService.GetPagedNavbarsAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var navbarsCount = await _navbarService.NavbarCountAsync();
            var navbarsPagedCount = await _navbarService.NavbarPagedCountAsync(portalId, language, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)navbarsCount, (int)navbarsPagedCount, navbars);
            return new DataTablesJsonResult(response, true);
        }

        public virtual async Task<IActionResult> NavbarList(long id /*current navbar id*/, int portalId, Entities.Language language, string term = null)
        {
            var navbars = await _navbarService.GetPagedNavbarsAsync(portalId, language, term, 0, int.MaxValue);
            var currentNavbarParentId = navbars.Where(x => x.Id == id).Select(x => x.ParentId).SingleOrDefault();
            navbars = navbars.Except(navbars.Where(x => x.Id == id).ToList()).ToList(); // Not allow to chosse current navbar as parent of itself

            var result = new { results = new List<object>() };
            foreach (var item in navbars)
            {
                if (currentNavbarParentId != null)
                {
                    result.results.Add(new { Id = item.Id, Text = item.Text, Selected = (currentNavbarParentId == item.Id) });
                }
                else
                {
                    result.results.Add(new { Id = item.Id, Text = item.Text });
                }
            }

            return Json(result);
        }

        public virtual IActionResult Add()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(NavbarViewModel navbar)
        {
            if (!ModelState.IsValid)
            {
                return View(navbar);
            }

            await _navbarService.AddOrUpdateNavbarAsync(navbar).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "navbar", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var navbar = await _navbarService.FindNavbarByIdAsync(id);

            if (navbar == null)
            {
                return RedirectToAction("index", "navbar", "manage");
            }

            if (navbar.ParentId != null && navbar.ParentId != 0)
            {
                var navbarParent = await _navbarService.GetParentOfNavbarAsync(navbar.ParentId ?? 0);

                if (navbarParent != null)
                {
                    ViewBag.ParentText = navbarParent.Text;
                }
            }

            var navbarViewModel = new NavbarViewModel
            {
                Id = navbar.Id,
                Text = navbar.Text,
                Url = navbar.Url,
                Icon = navbar.Icon,
                IsBlankUrlTarget = navbar.IsBlankUrlTarget,
                Priority = navbar.Priority,
                ParentId = navbar.ParentId,
                PortalId = navbar.PortalId,
                Language = navbar.Language
            };

            return View(navbarViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(NavbarViewModel navbar)
        {
            if (!ModelState.IsValid)
            {
                return View(navbar);
            }

            await _navbarService.AddOrUpdateNavbarAsync(navbar).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "navbar", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            await _navbarService.DeleteNavbarAsync(id);

            return Content("منو با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}
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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class PortalController : Controller
    {
        private readonly IPortalService _portalService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PortalController(IPortalService portalService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment hostingEnvironment)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language)
        {
            var portals = await _portalService.GetAllPortalsAsync().ConfigureAwait(false);

            foreach (var item in portals)
            {
                var baseOfCurrentDomain = _siteSettings.Value.DomainName;
                var pageHost = $"{item.PortalKey ?? "www"}.{baseOfCurrentDomain}";
                item.PortalLink = Url.Action("index", "home", new { area = "" }, Request.Scheme, pageHost);
            }

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            return Json(portals, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Include, ContractResolver = contractResolver });
        }

        public virtual IActionResult Add()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(PortalViewModel portal)
        {
            if (!ModelState.IsValid)
            {
                return View(portal);
            }

            var portalId = await _portalService.AddOrUpdatePortalAsync(portal).ConfigureAwait(false);

            var fileManagerpath = Path.Combine(_hostingEnvironment.WebRootPath, ContentManagement.Infrastructure.Constants.FilesManagerRootPath, "SubPortals", portalId.ToString());
            var imageManagerpath = Path.Combine(_hostingEnvironment.WebRootPath, ContentManagement.Infrastructure.Constants.ImagesManagerRootPath, "SubPortals", portalId.ToString());
            System.IO.Directory.CreateDirectory(fileManagerpath);
            System.IO.Directory.CreateDirectory(imageManagerpath);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "portal", "manage");
        }

        public async virtual Task<IActionResult> Update(int id)
        {
            var portal = await _portalService.FindPortalByIdAsync(id);

            if (portal == null)
            {
                return RedirectToAction("index", "portal", "manage");
            }

            var portalViewModel = new PortalViewModel
            {
                Id = portal.Id,
                PortalKey = portal.PortalKey,
                TitleFa = portal.TitleFa,
                HtmlTitleFa = portal.HtmlTitleFa,
                DescriptionFa = portal.DescriptionFa,
                TitleEn = portal.TitleEn,
                HtmlTitleEn = portal.HtmlTitleEn,
                DescriptionEn = portal.DescriptionEn,
                ShowInMainPortal = portal.ShowInMainPortal
            };

            return View(portalViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(PortalViewModel portal)
        {
            if (_siteSettings.Value.MainPortal.MainPortalId == portal.Id)
            {
                var key = nameof(PortalViewModel.PortalKey);
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                portal.PortalKey = null;
            }

            if (!ModelState.IsValid)
            {
                return View(portal);
            }

            await _portalService.AddOrUpdatePortalAsync(portal).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "portal", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (_siteSettings.Value.MainPortal.MainPortalId == id)
            {
                return Content("پرتال اصلی را نمی توانید حذف نمائید.", "text/html", System.Text.Encoding.UTF8);
            }

            await _portalService.DeletePortalAsync(id);

            return Content("پرتال با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckPortalKey(string portalKey, string initialPortalKey)
        {
            if (!string.IsNullOrEmpty(initialPortalKey) && portalKey.Trim() == initialPortalKey.Trim())
            {
                return Json(true);
            }

            return Json(!await _portalService.ValidatePortalKeyAsync(portalKey));
        }
    }
}
using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class FooterLinkController : Controller
    {
        private readonly IFooterLinkService _linkService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public FooterLinkController(IFooterLinkService linkService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index(long sid)
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, long sid)
        {
            var links = await _linkService.GetPagedLinksAsync(sid).ConfigureAwait(false);
            var linksCount = await _linkService.LinksCountAsync(sid);
            var linksPagedCount = linksCount;

            var response = DataTablesResponse.Create(request, (int)linksCount, (int)linksPagedCount, links);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(long sid)
        {
            var footerLinkViewModel = new FooterLinkViewModel();
            return View(footerLinkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(long sid, FooterLinkViewModel link)
        {
            if (!ModelState.IsValid)
            {
                return View(link);
            }

            await _linkService.AddOrUpdateLinkAsync(sid, link).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "footerlink", new { area = "manage", sid });
        }

        public async virtual Task<IActionResult> Update(long sid, long id)
        {
            var link = await _linkService.FindLinkByIdAsync(sid, id);

            if (link == null)
            {
                return RedirectToAction("index", "footerlink", new { area = "manage", sid });
            }

            var footerLinkViewModel = new FooterLinkViewModel
            {
                Id = link.Id,
                Text = link.Text,
                Url = link.Url,
                //Icon = link.Icon,
                //IconColor = link.IconColor,
                IsBlankUrlTarget = link.IsBlankUrlTarget,
                Priority = link.Priority,
            };

            return View(footerLinkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(long sid, FooterLinkViewModel link)
        {
            if (!ModelState.IsValid)
            {
                return View(link);
            }

            await _linkService.AddOrUpdateLinkAsync(sid, link).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "footerlink", new { area = "manage", sid });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long sid, long id)
        {
            await _linkService.DeleteLinkAsync(sid, id);

            return Content("لینک با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}
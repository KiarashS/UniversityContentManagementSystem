using ContentManagement.Common.GuardToolkit;
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
using ContentManagement.Common.ReflectionToolkit;
using System;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using ContentManagement.Common.WebToolkit.Attributes;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class LinkController : Controller
    {
        private readonly ILinkService _linkService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinkController(ILinkService linkService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env, IHtmlSanitizer htmlSanitizer, IActionContextAccessor actionContextAccessor)
        {
            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));

            _htmlSanitizer = htmlSanitizer;
            _htmlSanitizer.CheckArgumentIsNull(nameof(htmlSanitizer));

            _actionContextAccessor = actionContextAccessor;
            _actionContextAccessor.CheckArgumentIsNull(nameof(actionContextAccessor));
        }

        public virtual IActionResult Index()
        {
            var linkTypesList = new List<SelectListItem>() { new SelectListItem { Text = "", Selected = true } };
            var linkValues = Enum.GetValues(typeof(ContentManagement.Entities.LinkType)).Cast<ContentManagement.Entities.LinkType>();

            foreach (var item in linkValues)
            {
                var text = item.GetAttributeOfType<LinkTypeTextInAdminAttribute>().Description;
                linkTypesList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString() });
            }

            ViewBag.LinkTypes = linkTypesList;
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language, Entities.LinkType? linkType)
        {
            var links = await _linkService.GetPagedLinksAsync(portalId, linkType, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var linksCount = await _linkService.LinksCountAsync();
            var linksPagedCount = await _linkService.LinksPagedCountAsync(portalId, linkType, language, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)linksCount, (int)linksPagedCount, links);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            var linkViewModel = new LinkViewModel();
            return View(linkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(LinkViewModel link)
        {
            if (!ModelState.IsValid)
            {
                return View(link);
            }

            await _linkService.AddOrUpdateLinkAsync(link).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "link", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var link = await _linkService.FindLinkByIdAsync(id);

            if (link == null)
            {
                return RedirectToAction("index", "link", "manage");
            }

            var linkViewModel = new LinkViewModel
            {
                Id = link.Id,
                Text = link.Text,
                Url = link.Url,
                LinkType = link.LinkType,
                Icon = link.Icon,
                IconColor = link.IconColor,
                IsBlankUrlTarget = link.IsBlankUrlTarget,
                Priority = link.Priority,
                PortalId = link.PortalId,
                Language = link.Language
            };

            return View(linkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(LinkViewModel link)
        {
            if (!ModelState.IsValid)
            {
                return View(link);
            }

            await _linkService.AddOrUpdateLinkAsync(link).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "link", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            await _linkService.DeleteLinkAsync(id);

            return Content("لینک با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}
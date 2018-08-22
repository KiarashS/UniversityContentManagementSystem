using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.Services.Seo;
using ContentManagement.ViewModels.Settings;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ContentManagement.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly SeoService _seoService;

        public PageController(IPageService pageService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, SeoService seoService)
        {
            _pageService = pageService;
            _pageService.CheckArgumentIsNull(nameof(pageService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));
        }


        public async Task<IActionResult> Index(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            var portalKey = _requestService.PortalKey();
            var language = _requestService.CurrentLanguage().Language;
            var page = await _pageService.GetPageDetails(portalKey, language, slug);

            if (page == null)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            _seoService.Title = page.Title;
            _seoService.MetaDescription = page.GetSummary;
            _seoService.MetaKeywords = page.Keywords;

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = page.Title,
                Order = 2,
                GlyphIcon = "far fa-file-alt"
            });

            return View(page);
        }
    }
}
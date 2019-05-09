using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using ContentManagement.Services.Seo;
using DNTBreadCrumb.Core;
using ContentManagement.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ContentManagement.Controllers
{
    public partial class SearchController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly SeoService _seoService;
        private readonly IUrlUtilityService _urlUtilityService;

        public SearchController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, SeoService seoService, IUrlUtilityService urlUtilityService)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));

            _urlUtilityService = urlUtilityService;
            _urlUtilityService.CheckArgumentIsNull(nameof(urlUtilityService));
        }


        public async virtual Task<IActionResult> Index(string q, int? page)
        {
            if (!page.HasValue || page < 1)
            {
                page = 1;
            }

            var vm = new SearchViewModel();
            var portalKey = _requestService.PortalKey();
            var language = _requestService.CurrentLanguage().Language;
            vm.Page = page.Value;
            vm.PageSize = _siteSettings.Value.PagesSize.SearchSize;
            //vm.Start = (vm.Page - 1) * vm.PageSize;
            vm.Start = (vm.PageSize * vm.Page) - vm.PageSize;
            q = System.Net.WebUtility.HtmlEncode(q);

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = _sharedLocalizer["Search"],
                Order = 2,
                GlyphIcon = "fas fa-search"
            });

            if (string.IsNullOrEmpty(q))
            {
                _seoService.Title = _sharedLocalizer["Search"];
                return View(vm);
            }

            vm.Q = q.Trim();
            vm.ContentsViewModel = await _contentService.GetSearchResultsAsync(portalKey, language, vm.Q, vm.Start, vm.PageSize);
            vm.TotalCount = await _contentService.GetSearchResultsCountAsync(portalKey, language, vm.Q);

            foreach (var item in vm.ContentsViewModel)
            {
                item.Link = _urlUtilityService.GenerateUrl(portalKey, item.Id, item.Title, Url, scheme: Request.Scheme);
            }

            _seoService.Title = _sharedLocalizer["Search"] + " " + vm.Q;
            //this.AddBreadCrumb(new BreadCrumb
            //{
            //    Title = "a",
            //    Order = 3,
            //    GlyphIcon = "fas fa-list-alt"
            //});

            return View(vm);
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> SearchAutoComplete(string q)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var currentPortalKey = _requestService.PortalKey();
            var size = _siteSettings.Value.PagesSize.SearchAutoCompleteSize;
            var vm = new SearchAutoCompleteResult();
            IList<SearchAutoCompleteViewModel> results;
            q = System.Net.WebUtility.HtmlEncode(q);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
            jsonSerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };

            if (string.IsNullOrEmpty(q))
            {
                return Json(vm, jsonSerializerSettings);
            }

            try
            {
                results = await _contentService.GetSearchAutoCompleteAsync(currentPortalKey, currentLanguage, q, size);
            }
            catch (System.Exception)
            {
                vm.Status = false;
                vm.Error = _sharedLocalizer["Sorry An Error Occurred"];
                return Json(vm, jsonSerializerSettings);
            }

            if (!results.Any() || results == null)
            {
                vm.Status = false;
                return Json(vm, jsonSerializerSettings);
            }

            var contentsImagePath = ContentManagement.Infrastructure.Constants.ContentsRootPath;
            foreach (var item in results)
            {
                vm.Data.Results.Add(new SearchAutoCompleteItem {
                    Id = item.Id,
                    Title = item.Title,
                    Text = item.Text,
                    Imagename = !string.IsNullOrEmpty(item.Imagename) ? $"/{contentsImagePath}/{item.Imagename}" : "/statics/logo.png",
                    ContentType = item.TypeOfContent,
                    Link = _urlUtilityService.GenerateUrl(currentPortalKey, item.Id, item.Title, Url, scheme: Request.Scheme),
                    IsArchive = item.IsArchive,
                    IsLastItem = results.Last() == item
                });
            }

            vm.ResultsCount = results.Count;

            return Json(vm, jsonSerializerSettings);
        }
    }
}

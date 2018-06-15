using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Common.ReflectionToolkit;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using ContentManagement.Services.Seo;

namespace ContentManagement.Controllers
{
    public partial class ContentController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer _sharedLocalizer;
        private readonly SeoService _seoService;

        public ContentController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, SeoService seoService)
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
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> FetchNews(ContentType t = ContentType.News, bool favorite = false)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var currentPortalKey = _requestService.PortalKey();
            int size;
            IList<ContentManagement.ViewModels.ContentsViewModel> vm;
            
            if (favorite)
            {
                ViewData["NewsQuery"] = "?favorite=true";
                ViewData["IsFavorite"] = true;
                size = _siteSettings.Value.PagesSize.FavoritesTabSize;
                vm = await _contentService.GetFavoritesAsync(currentPortalKey, currentLanguage, 0, size).ConfigureAwait(false);
            }
            else if ( t == ContentType.UpcomingEvent)
            {
                ViewData["NewsQuery"] = "?t=upcomingevent";
                ViewData["IsFavorite"] = false;
                size = _siteSettings.Value.PagesSize.UpcomingEventsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.UpcomingEvent, 0, size).ConfigureAwait(false);
            }
            else
            {
                ViewData["NewsQuery"] = "?t=news";
                ViewData["IsFavorite"] = false;
                size = _siteSettings.Value.PagesSize.NewsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.News, 0, size).ConfigureAwait(false);
            }

            return PartialView("_News", vm);
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> FetchContents(ContentType? t, bool otherContents = false)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var currentPortalKey = _requestService.PortalKey();
            int size;
            IList<ContentManagement.ViewModels.ContentsViewModel> vm;

            ViewData["IsOtherContents"] = false;

            if (otherContents)
            {
                ViewData["IsOtherContents"] = true;
                size = _siteSettings.Value.PagesSize.OtherContentsTabSize;

                if (t.HasValue)
                {
                    vm = await _contentService.GetOtherContentsAsync(currentPortalKey, t.Value, currentLanguage, 0, size).ConfigureAwait(false);
                    ViewData["ContentsQuery"] = "?othercontents=true&t=" + t.Value.ToString().ToLowerInvariant();
                }
                else
                {
                    vm = await _contentService.GetOtherContentsAsync(currentPortalKey, null, currentLanguage, 0, size).ConfigureAwait(false);
                    ViewData["ContentsQuery"] = "?othercontents=true";
                }

                var contentsVisibilityViewModel = await _contentService.CheckContentsVisibility(_requestService.PortalKey(), currentLanguage);
                var otherContentsItems = new List<SelectListItem>();

                foreach (var item in contentsVisibilityViewModel.Where(x => x.IsVisible).ToList())
                {
                    if (item.ContentType != ContentType.Education &&
                        item.ContentType != ContentType.Form &&
                        item.ContentType != ContentType.EducationalCalendar &&
                        item.ContentType != ContentType.StudentAndCultural &&
                        item.ContentType != ContentType.PoliticalAndIdeological)
                    {
                        var text = "";
                        var value = item.ContentType.ToString().ToLowerInvariant();

                        if (currentLanguage == Entities.Language.EN)
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                        }
                        else
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                        }

                        otherContentsItems.Add(new SelectListItem
                        {
                            Text = text,
                            Value = value,
                            Selected = (t.HasValue && t == item.ContentType)
                        });
                    }
                }

                ViewData["ContentTypes"] = otherContentsItems;
            }
            else if (t == ContentType.Form)
            {
                ViewData["ContentsQuery"] = "?t=form";
                size = _siteSettings.Value.PagesSize.FormsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.Form, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.EducationalCalendar)
            {
                ViewData["ContentsQuery"] = "?t=educationalcalendar";
                size = _siteSettings.Value.PagesSize.EducationalCalendarsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.EducationalCalendar, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.StudentAndCultural)
            {
                ViewData["ContentsQuery"] = "?t=studentandcultural";
                size = _siteSettings.Value.PagesSize.StudentAndCulturalsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.StudentAndCultural, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.PoliticalAndIdeological)
            {
                ViewData["ContentsQuery"] = "?t=politicalandideological";
                size = _siteSettings.Value.PagesSize.PoliticalAndIdeologicalsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.PoliticalAndIdeological, 0, size).ConfigureAwait(false);
            }
            else
            {
                ViewData["ContentsQuery"] = "?t=education";
                size = _siteSettings.Value.PagesSize.EducationsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.Education, 0, size).ConfigureAwait(false);
            }

            return PartialView("_Contents", vm);
        }

        public async virtual Task<IActionResult> Details(long id, string title)
        {
            return Content("");
        }
    }
}

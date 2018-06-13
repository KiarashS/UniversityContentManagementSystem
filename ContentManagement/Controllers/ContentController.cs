using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using DNTCommon.Web.Core;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;

namespace ContentManagement.Controllers
{
    public partial class ContentController : Controller
    {
        private readonly IContentService _contentService;
        protected readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        protected readonly IRequestService _requestService;

        public ContentController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
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
                size = _siteSettings.Value.PagesSize.FavoritesSize;
                vm = await _contentService.GetFavoritesAsync(currentPortalKey, currentLanguage, 0, size);
            }
            else if ( t == ContentType.UpcomingEvent)
            {
                ViewData["NewsQuery"] = "?t=upcomingevent";
                size = _siteSettings.Value.PagesSize.UpcomingEventsSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.UpcomingEvent, 0, size);
            }
            else
            {
                ViewData["NewsQuery"] = "?t=news";
                size = _siteSettings.Value.PagesSize.NewsSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.News, 0, size);
            }

            return PartialView("_news", vm);
        }
    }
}

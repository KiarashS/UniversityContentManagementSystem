using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.ViewModels;
using System.Linq;

namespace ContentManagement.ViewComponents
{
    public class LatestNewsOrEventsOrFavorites : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public LatestNewsOrEventsOrFavorites(IContentService contentService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var newsSize = _siteSettings.Value.PagesSize.NewsTabSize;
            var eventsSize = _siteSettings.Value.PagesSize.UpcomingEventsTabSize;
            var favoriteContentsSize = _siteSettings.Value.PagesSize.FavoritesTabSize;
            var vm = new LatestNewsOrEventsOrFavoritesViewModel();

            vm.NewsOrEventsOrFavoritesViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.News, 0, newsSize).ConfigureAwait(false);

            vm.IsExistNews = vm.NewsOrEventsOrFavoritesViewModel.Any(x => x.ContentType == Entities.ContentType.News);
            vm.IsExistEvent = await _contentService.IsExistContent(_requestService.PortalKey(), currentLanguage, Entities.ContentType.UpcomingEvent).ConfigureAwait(false);
            vm.IsExistAnnouncement = await _contentService.IsExistContent(_requestService.PortalKey(), currentLanguage, Entities.ContentType.Announcement).ConfigureAwait(false);
            vm.IsExistFavorite = await _contentService.IsExistFavorite(_requestService.PortalKey(), null, currentLanguage).ConfigureAwait(false);


            if (!vm.IsExistNews && vm.IsExistAnnouncement) // just for tab initialize, in order
            {
                vm.NewsOrEventsOrFavoritesViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.UpcomingEvent, 0, eventsSize).ConfigureAwait(false);
            }
            else if (!vm.IsExistNews && vm.IsExistFavorite)
            {
                vm.NewsOrEventsOrFavoritesViewModel = await _contentService.GetFavoritesAsync(_requestService.PortalKey(), null, currentLanguage, 0, favoriteContentsSize).ConfigureAwait(false);
            }


            if (vm.IsExistEvent && !vm.IsExistNews)
            {
                ViewData["NewsQuery"] = "?t=upcomingevent";
                ViewData["IsFavorite"] = false;
            }
            else if (vm.IsExistFavorite && !vm.IsExistEvent && !vm.IsExistNews)
            {
                ViewData["NewsQuery"] = "?favorite=true";
                ViewData["IsFavorite"] = true;
            }
            else
            {
                ViewData["NewsQuery"] = "?t=news";
                ViewData["IsFavorite"] = false;
            }


            return View(vm);
        }
    }
}

using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class LatestAnnouncements : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public LatestAnnouncements(IContentService contentService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
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
            var contentsSize = _siteSettings.Value.PagesSize.AnnouncementTabSize;

            var vm = await _contentService.GetContentsAsync(_requestService.PortalKey(), Entities.ContentType.Announcement, currentLanguage, 0, contentsSize).ConfigureAwait(false);

            return View(vm);
        }
    }
}

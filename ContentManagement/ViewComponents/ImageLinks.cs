using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class ImageLinks : ViewComponent
    {
        private readonly IImageLinkService _imageLinkService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public ImageLinks(IImageLinkService imageLinkService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _imageLinkService = imageLinkService;
            _imageLinkService.CheckArgumentIsNull(nameof(imageLinkService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var imageLinksSize = _siteSettings.Value.PagesSize.ImageLinkSize;
            var vm = await _imageLinkService.GetImageLinksAsync(_requestService.PortalKey(), currentLanguage, imageLinksSize);

            return View(vm);
        }
    }
}

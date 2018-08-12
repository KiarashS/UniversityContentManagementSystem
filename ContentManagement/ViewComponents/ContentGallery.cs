using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class ContentGallery : ViewComponent
    {
        private readonly IContentGalleryService _galleryService;
        //private readonly IRequestService _requestService;
        //private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public ContentGallery(IContentGalleryService galleryService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _galleryService = galleryService;
            _galleryService.CheckArgumentIsNull(nameof(galleryService));

            //_requestService = requestService;
            //_requestService.CheckArgumentIsNull(nameof(requestService));

            //_siteSettings = siteSettings;
            //_siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contentId = long.Parse(ViewContext.RouteData.Values["id"].ToString());
            var vm = await _galleryService.GetContentGalleryAsync(contentId);

            return View(vm);
        }
    }
}

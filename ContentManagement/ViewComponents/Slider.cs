using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class Slider : ViewComponent
    {
        private readonly ISlideService _slideService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public Slider(ISlideService slideService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _slideService = slideService;
            _slideService.CheckArgumentIsNull(nameof(slideService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var sliderSize = _siteSettings.Value.PagesSize.SliderSize;
            var vm = await _slideService.GetPortalSlidesAsync(_requestService.PortalKey(), currentLanguage, sliderSize);

            return View(vm);
        }
    }
}

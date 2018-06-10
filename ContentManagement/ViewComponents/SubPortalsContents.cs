using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class SubPortalsContents : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public SubPortalsContents(IContentService contentService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
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
            var contentsSize = _siteSettings.Value.PagesSize.SubPortalsContentsSize;
            var vm = await _contentService.GetSubPortalsContentsAsync(currentLanguage, contentsSize);

            return View(vm);
        }
    }
}

using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class ExternalLinks : ViewComponent
    {
        private readonly ILinkService _linkService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public ExternalLinks(ILinkService linkService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var linksSize = _siteSettings.Value.PagesSize.ExternalLinksSize;
            var vm = await _linkService.GetLinksAsync(_requestService.PortalKey(), currentLanguage, Entities.LinkType.External, linksSize);

            return View(vm);
        }
    }
}

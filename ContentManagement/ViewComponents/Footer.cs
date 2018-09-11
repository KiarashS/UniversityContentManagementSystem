using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class Footer : ViewComponent
    {
        private readonly IFooterSectionService _sections;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public Footer(IFooterSectionService sections, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _sections = sections;
            _sections.CheckArgumentIsNull(nameof(sections));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var footerLinksSize = _siteSettings.Value.PagesSize.FooterLinksSize;
            //var vm = await _linkService.GetLinksAsync(_requestService.PortalKey(), _requestService.CurrentLanguage().Language, Entities.LinkType.Footer, footerLinksSize);

            var footerSectionsSize = _siteSettings.Value.PagesSize.FooterSectionsSize;
            var footerLinksSize = _siteSettings.Value.PagesSize.FooterLinksSize;
            var vm = await _sections.GetSectionAndLinksAsync(_requestService.PortalKey(), _requestService.CurrentLanguage().Language, footerSectionsSize, footerLinksSize);

            return View(vm);
        }
    }
}

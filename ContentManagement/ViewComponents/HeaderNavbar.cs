using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class HeaderNavbar : ViewComponent
    {
        private readonly INavbarService _navbarService;
        private readonly ILinkService _linkService;
        private readonly IRequestService _requestService;

        public HeaderNavbar(INavbarService navbarService, ILinkService linkService, IRequestService requestService)
        {
            _navbarService = navbarService;
            _navbarService.CheckArgumentIsNull(nameof(navbarService));

            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var portalKey = _requestService.PortalKey();
            var vm = await _navbarService.GetHeaderNavbarsAsync(portalKey, currentLanguage);
            ViewBag.NavbarHasQuickLinks = await _linkService.HasLink(portalKey, currentLanguage, Entities.LinkType.Quick);

            return View(vm);
        }
    }
}

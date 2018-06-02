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
        private readonly IRequestService _requestService;

        public HeaderNavbar(INavbarService navbarService, IRequestService requestService)
        {
            _navbarService = navbarService;
            _navbarService.CheckArgumentIsNull(nameof(navbarService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var vm = await _navbarService.GetHeaderNavbarsAsync(_requestService.PortalKey(), currentLanguage);

            return View(vm);
        }
    }
}

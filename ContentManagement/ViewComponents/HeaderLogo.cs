using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.ViewModels;
using DNTPersianUtils.Core;

namespace ContentManagement.ViewComponents
{
    public class HeaderLogo : ViewComponent
    {
        private readonly IPortalService _portalService;
        private readonly IRequestService _requestService;

        public HeaderLogo(IPortalService portalService, IRequestService requestService)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var title = await _portalService.GetPortalTitleAsync(_requestService.PortalKey(), currentLanguage);

            var vm = new HeaderLogoViewModel
            {
                PortalTitle = title,
                TodayDate = getTodayDate(currentLanguage)
            };

            return View(vm);
        }

        private string getTodayDate(Entities.Language language)
        {
            if (language == Entities.Language.EN)
            {
                return DateTimeOffset.UtcNow.ToString("dddd, d MMMM yyyy");
            }
            else
            {
                return DateTimeOffset.UtcNow.ToPersianDateTextify();
            }
        }
    }
}

﻿using ContentManagement.Services.Contracts;
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
        private readonly ILinkService _linkService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public Footer(ILinkService linkService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
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
            var footerLinksSize = _siteSettings.Value.PagesSize.FooterLinksSize;
            var vm = await _linkService.GetLinksAsync(_requestService.PortalKey(), _requestService.CurrentLanguage().Language, Entities.LinkType.Footer, footerLinksSize);

            return View(vm);
        }
    }
}

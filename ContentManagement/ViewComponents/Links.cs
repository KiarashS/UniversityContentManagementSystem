﻿using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.Entities;
using System.Linq;

namespace ContentManagement.ViewComponents
{
    public class Links : ViewComponent
    {
        private readonly ILinkService _linkService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public Links(ILinkService linkService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
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
            //var linksSize = _siteSettings.Value.PagesSize.ExternalLinksSize;
            var selectedLinkType = LinkType.Useful; // default value
            var linkVisibility = await _linkService.CheckLinksVisibility(_requestService.PortalKey(), currentLanguage); // x => (x.LinkType == LinkType.Useful || x.LinkType == LinkType.ElectronicService || x.LinkType == LinkType.ElectronicResource))

            if (linkVisibility.Any(x => x.LinkType == LinkType.Useful && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.ElectronicService && x.IsVisible))
            {
                selectedLinkType = LinkType.ElectronicService;
            }
            else if (linkVisibility.Any(x => x.LinkType == LinkType.Useful && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.ElectronicService && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.ElectronicResource && x.IsVisible))
            {
                selectedLinkType = LinkType.ElectronicResource;
            }
            else if (linkVisibility.Any(x => x.LinkType == LinkType.Useful && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.ElectronicService && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.ElectronicResource && !x.IsVisible) && linkVisibility.Any(x => x.LinkType == LinkType.Hospital && x.IsVisible))
            {
                selectedLinkType = LinkType.Hospital;
            }
            
            var vm = await _linkService.GetLinksAsync(_requestService.PortalKey(), currentLanguage, selectedLinkType, int.MaxValue);
            ViewBag.LinkVisibility = linkVisibility;

            return View(vm);
        }
    }
}

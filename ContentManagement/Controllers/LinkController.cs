using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using DNTCommon.Web.Core;

namespace ContentManagement.Controllers
{
    public partial class LinkController : Controller
    {
        private readonly ILinkService _linkService;
        protected readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        protected readonly IRequestService _requestService;

        public LinkController(ILinkService linkService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _linkService = linkService;
            _siteSettings = siteSettings;
            _requestService = requestService;
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> FetchLinks(LinkType tab = LinkType.Useful)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            //var linksSize = _siteSettings.Value.PagesSize.ExternalLinksSize;
            var vm = await _linkService.GetLinksAsync(_requestService.PortalKey(), currentLanguage, tab, int.MaxValue);

            return PartialView("_links", vm);
        }
    }
}

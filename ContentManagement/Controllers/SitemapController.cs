using System;
using System.Collections.Generic;
using System.Text;
using Boilerplate.AspNetCore.Filters;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ContentManagement.Services;

namespace ContentManagement.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IPortalService _portalService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _environment;
        private readonly IRequestService _requestService;

        public SitemapController(IHostingEnvironment environment, IPortalService portalService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _environment = environment;
            _environment.CheckArgumentIsNull(nameof(environment));

            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
        }


        [Route("/sitemap")]
        public async Task<IActionResult> Sitemap()
        {
            var items = new List<SitemapItem>();
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            var portalsKey = await _portalService.GetPortalsKeyAsync().ConfigureAwait(false);

            // Index of current url is added by default in DNT SitemapResult, So we remove that from portals key list
            portalsKey = portalsKey.Where(x => x.PortalKey != null).ToList();

            foreach (var item in portalsKey)
            {
                var pageHost = $"{item.PortalKey ?? "www"}.{baseOfCurrentDomain}";

                items.Add(new SitemapItem {
                    Url = Url.Action("Index", typeof(HomeController).ControllerName(), null, this.HttpContext.Request.Scheme, pageHost),
                    LastUpdatedTime = DateTime.Now, //DateTimeOffset.UtcNow,
                    Priority = 1.0M
                });
            }

            // Add contact page
            var contactPagePortalKey = string.IsNullOrEmpty(_requestService.PortalKey()) ? "www" : _requestService.PortalKey();
            var contactPageHost = $"{contactPagePortalKey}.{baseOfCurrentDomain}";
            items.Add(new SitemapItem
            {
                Url = Url.Action("Index", typeof(ContactController).ControllerName(), null, this.HttpContext.Request.Scheme, $"www.{baseOfCurrentDomain}"),
                LastUpdatedTime = DateTime.Now, //DateTimeOffset.UtcNow,
                Priority = 1.0M
            });

            return new SitemapResult(items);
        }

        [Route("/robots.txt")] // Autodiscovery of /sitemap
        [NoTrailingSlash]
        public string RobotsTxt()
        {
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            var pageHost = $"www.{baseOfCurrentDomain}";

            string siteMapFullUrl = Url.Action(nameof(Sitemap), typeof(SitemapController).ControllerName(), null, this.HttpContext.Request.Scheme, pageHost);

            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            //sb.AppendLine("Disallow: /login/");
            //sb.AppendLine("Disallow: /manage/*");
            sb.AppendLine($"Sitemap: {siteMapFullUrl}");
            return sb.ToString();
        }

        //[Route("/BingSiteAuth.xml")] // Autodiscovery of /BingSiteAuth.xml
        //[NoTrailingSlash]
        //public IActionResult BingSiteAuth()
        //{
        //    var file = Path.Combine(_environment.WebRootPath, "BingSiteAuth.xml");
        //    return PhysicalFile(file, "application/xml");
        //}
    }
}

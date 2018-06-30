﻿using System;
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

namespace ContentManagement.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IPortalService _portalService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public SitemapController(IPortalService portalService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
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
            //sb.AppendLine("Disallow:");
            sb.AppendLine($"Sitemap: {siteMapFullUrl}");
            return sb.ToString();
        }
    }
}

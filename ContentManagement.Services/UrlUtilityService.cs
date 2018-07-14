using ContentManagement.ViewModels.Settings;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;

namespace ContentManagement.Services
{
    public interface IUrlUtilityService
    {
        string Target(string url, bool isBlankUrlTarget = true);
        string ExternalClassName(string className = "external");
        bool IsExternal(string url);
        string GenerateUrl(string portalKey, long id, string title, IUrlHelper url, string scheme, string routeName = "default", string area = null, string controller = "content", string action = "details", bool forceAbsoluteUrl = false);
        string GeneratePageUrl(string portalKey, string slug, IUrlHelper url, string scheme, string routeName = "pageRoute", string area = null, string controller = "page", string action = "index", bool forceAbsoluteUrl = false);
    }

    public class UrlUtilityService : IUrlUtilityService
    {
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlUtilityService(IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IHttpContextAccessor httpContextAccessor)
        {
            _siteSettings = siteSettings ?? throw new ArgumentNullException(nameof(siteSettings));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string Target(string url, bool isBlankUrlTarget = true)
        {
            if (IsExternal(url) || isBlankUrlTarget)
            {
                return "_blank";
            }

            return "_self";
        }

        public string ExternalClassName(string className = "external")
        {
            return className;
        }

        public bool IsExternal(string url)
        {
            if (string.IsNullOrEmpty(url) ||
                url.StartsWith("javascript", StringComparison.InvariantCultureIgnoreCase) ||
                url.StartsWith("#", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            
            var baseDomain = _siteSettings.Value.DomainName;

            if (isAbsoluteUrl(url) && !new Uri(url).Host.Contains(baseDomain))
            {
                return true;
            }

            return false;
        }

        bool isAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public string GenerateUrl(string portalKey, long id, string title, IUrlHelper url, string scheme, string routeName = "default", string area = null, string controller = "content", string action = "detail", bool forceAbsoluteUrl = false)
        {
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            title = ContentManagement.Common.WebToolkit.Slug.SeoFriendlyTitle(title);
            
            if (string.Equals(portalKey, _requestService.PortalKey()) && !forceAbsoluteUrl)
            {
                return url.RouteUrl(routeName, new { controller, action, area, id, title = !string.IsNullOrEmpty(title) ? WebUtility.UrlDecode(title) : null });
            }
            else if (_httpContextAccessor.HttpContext.IsLocal())
            {
                baseOfCurrentDomain = _httpContextAccessor.HttpContext.Request.Host.Port.HasValue ? "localhost:" + _httpContextAccessor.HttpContext.Request.Host.Port.Value.ToString() : "localhost";
                var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

                return url.RouteUrl(routeName, new { controller, action, area, id, title = !string.IsNullOrEmpty(title) ? WebUtility.UrlDecode(title) : null }, scheme, pageHost);
            }
            else
            {
                var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

                return url.RouteUrl(routeName, new { controller, action, area, id, title = !string.IsNullOrEmpty(title) ? WebUtility.UrlDecode(title) : null }, scheme, pageHost);
            }
        }

        public string GeneratePageUrl(string portalKey, string slug, IUrlHelper url, string scheme, string routeName = "pageRoute", string area = null, string controller = "page", string action = "index", bool forceAbsoluteUrl = false)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return "#";
            }

            var baseOfCurrentDomain = _siteSettings.Value.DomainName;

            if (string.Equals(portalKey, _requestService.PortalKey()) && !forceAbsoluteUrl)
            {
                return url.RouteUrl(routeName, new { controller, action, area, slug = WebUtility.UrlDecode(slug)});
            }
            else if (_httpContextAccessor.HttpContext.IsLocal())
            {
                baseOfCurrentDomain = _httpContextAccessor.HttpContext.Request.Host.Port.HasValue ? "localhost:" + _httpContextAccessor.HttpContext.Request.Host.Port.Value.ToString() : "localhost";
                var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

                return url.RouteUrl(routeName, new { controller, action, area, slug =WebUtility.UrlDecode(slug) }, scheme, pageHost);
            }
            else
            {
                var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

                return url.RouteUrl(routeName, new { controller, action, area, slug = WebUtility.UrlDecode(slug) }, scheme, pageHost);
            }
        }
    }
}

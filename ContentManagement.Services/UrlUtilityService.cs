using ContentManagement.ViewModels.Settings;
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
        string GenerateUrl(string portalKey, long id, string title, IUrlHelper url, string scheme, string routeName = "default", string area = null, string controller = "content", string action = "detail");
    }

    public class UrlUtilityService : IUrlUtilityService
    {
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public UrlUtilityService(IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _siteSettings = siteSettings ?? throw new ArgumentNullException(nameof(siteSettings));
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

        public string GenerateUrl(string portalKey, long id, string title, IUrlHelper url, string scheme, string routeName = "default", string area = null, string controller = "content", string action = "detail")
        {
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

            return url.RouteUrl(routeName, new { controller, action, area, id, title = !string.IsNullOrEmpty(title) ? WebUtility.UrlDecode(title) : null }, scheme, pageHost);
        }
    }
}

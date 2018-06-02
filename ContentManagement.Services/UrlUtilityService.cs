using ContentManagement.ViewModels.Settings;
using Microsoft.Extensions.Options;
using System;

namespace ContentManagement.Services
{
    public interface IUrlUtilityService
    {
        string Target(string url, bool isBlankUrlTarget = true);
        string ExternalClassName(string className = "external");
        bool IsExternal(string url);
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
    }
}

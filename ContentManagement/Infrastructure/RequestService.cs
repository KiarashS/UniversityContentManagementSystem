using ContentManagement.Common.WebToolkit;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;

namespace ContentManagement.Infrastructure
{
    public interface IRequestService
    {
        RequestLanguage CurrentLanguage();
        bool IsLocal();
        string CurrentSubDomain();
        bool IsSubPortal();
    }

    public class RequestService : IRequestService
    {
        private readonly string localhost = "localhost";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public RequestService(IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _siteSettings = siteSettings ?? throw new ArgumentNullException(nameof(siteSettings));
        }

        public RequestLanguage CurrentLanguage()
        {
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();

            if (requestCulture.RequestCulture.UICulture.Equals(new CultureInfo("en-US")))
            {
                return new RequestLanguage
                {
                    Language = Language.EN,
                    Direction = Direction.LeftToRight
                };
            }

            return new RequestLanguage
            {
                Language = Language.FA,
                Direction = Direction.RightToLeft
            };
        }

        public string CurrentSubDomain()
        {
            var siteSettings = _siteSettings.Value;
            var subDomain = string.Empty;

            var host = _httpContextAccessor.HttpContext.Request.Host.Host;

            if (!string.IsNullOrWhiteSpace(host))
            {
                // subDomain = host.Split('.')[0];
                if (_httpContextAccessor.HttpContext.IsLocal())
                {
                    subDomain = host.Substring(0, host.IndexOf(localhost)).Trim().ToLowerInvariant();
                }
                else
                {
                    subDomain = host.Substring(0, host.IndexOf(siteSettings.DomainName)).Trim().ToLowerInvariant();
                }
                
                // remove last trailing .
                if (subDomain.EndsWith('.'))
                {
                    subDomain = subDomain.Remove(subDomain.Length - 1);
                }
            }

            if (subDomain.Length > 0 && (subDomain == siteSettings.DomainName.Split('.')[0].ToLowerInvariant() || subDomain == localhost))
            {
                subDomain = string.Empty;
            }

            if (subDomain.Trim().Length != 0)
            {
                return subDomain.Trim().ToLowerInvariant();
            }
            else
            {
                return null;
            }
        }

        public bool IsLocal()
        {
            return _httpContextAccessor.HttpContext.Request.IsLocal();
        }

        public bool IsSubPortal()
        {
            var subDomain = CurrentSubDomain();
            var siteSettings = _siteSettings.Value;

            return  !string.IsNullOrEmpty(subDomain) && subDomain.Length != 0 && !siteSettings.SubDomainsBanList.Any(sub => sub == subDomain);
        }
    }

    public class RequestLanguage
    {
        public Language Language { get; set; }
        public Direction Direction { get; set; }
    }
}

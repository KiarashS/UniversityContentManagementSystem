using ContentManagement.Common.WebToolkit;
using ContentManagement.Entities;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;

namespace ContentManagement.Services
{
    public interface IRequestService
    {
        RequestLanguage CurrentLanguage();
        bool IsLocal();
        string CurrentSubDomain();
        string CurrentPortal();
        bool IsSubPortal();
        bool IsRtl();
        bool IsLtr();
        string PortalKey();
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
                    if (host.IndexOf(localhost) >= 0)
                    {
                        subDomain = host.Substring(0, host.IndexOf(localhost)).Trim().ToLowerInvariant();
                    }
                    else
                    {
                        _httpContextAccessor
                            .HttpContext
                            .Response
                            .Redirect($"{_httpContextAccessor.HttpContext.Request.Scheme}://{localhost}", true);
                    }
                }
                else
                {
                    subDomain = host.Substring(0, host.IndexOf(siteSettings.DomainName)).Trim().ToLowerInvariant();
                }
                
                // remove last trailing .
                if (subDomain.EndsWith("."))
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

        public string CurrentPortal()
        {
            var currentSubDomain = CurrentSubDomain();

            if (string.IsNullOrEmpty(currentSubDomain) || currentSubDomain == "www")
            {
                return null;
            }

            return currentSubDomain;
        }

        public bool IsLocal()
        {
            return _httpContextAccessor.HttpContext.Request.IsLocal();
        }

        public bool IsSubPortal()
        {
            var subDomain = CurrentSubDomain();
            var subDomainsBanList = _siteSettings.Value.SubDomainsBanList;

            return  !string.IsNullOrEmpty(subDomain) && subDomain.Length != 0 && !subDomainsBanList.Any(sub => sub == subDomain);
        }

        public bool IsRtl()
        {
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();

            return requestCulture.RequestCulture.UICulture.TextInfo.IsRightToLeft;
        }

        public bool IsLtr()
        {
            return !IsRtl();
        }

        public string PortalKey()
        {
            string portalKey = null;

            if (IsSubPortal())
            {
                portalKey = CurrentSubDomain();
            }

            return portalKey;
        }
    }

    public class RequestLanguage
    {
        public Language Language { get; set; }
        public Direction Direction { get; set; }
    }
}

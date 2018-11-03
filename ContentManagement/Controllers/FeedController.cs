using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Hosting;

namespace ContentManagement.Controllers
{
    public partial class FeedController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IUrlUtilityService _urlUtilityService;
        private readonly IPortalService _portalService;

        public FeedController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, IPortalService portalService, IUrlUtilityService urlUtilityService)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));

            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _urlUtilityService = urlUtilityService;
            _urlUtilityService.CheckArgumentIsNull(nameof(urlUtilityService));
        }

        public async virtual Task<IActionResult> Rss(ContentType? t)
        {
            var rssItems = new List<FeedItem>();
            var settings = _siteSettings.Value;
            var portalKey = _requestService.PortalKey();
            var baseOfCurrentDomain = settings.DomainName;
            var rssSize = settings.PagesSize.RssSize;
            var contents = await _contentService.GetRssResult(portalKey, t, rssSize).ConfigureAwait(false);
            var portalInfo = await _portalService.FindPortalByKeyAsync(portalKey).ConfigureAwait(false);
            var pageHost = $"{portalKey ?? "www"}.{baseOfCurrentDomain}";

            foreach (var item in contents)
            {
                rssItems.Add(new FeedItem
                {
                    Title = item.Title,
                    AuthorName = $"{portalInfo.TitleFa} | {portalInfo.TitleEn}",
                    Categories = item.Categories,
                    Content = item.Content,
                    Url = Url.RouteUrl("default", new { action = "details", controller = "content", id = item.Id, title = Common.WebToolkit.Slug.SeoFriendlyTitle(item.Title), culture = "en-US", ui_culture = item.Language == Language.FA ? "fa-IR" : "en-US" }, this.HttpContext.Request.Scheme, pageHost).Replace("ui_culture", "ui-culture"),
                    PublishDate = item.PublishDate,
                    LastUpdatedTime = item.PublishDate
                });
            }

            var channel = new FeedChannel
            {
                FeedTitle = $"{portalInfo.TitleFa} | {portalInfo.TitleEn}",
                FeedDescription = $"{portalInfo.DescriptionFa} | {portalInfo.DescriptionEn}",
                FeedCopyright = $"{settings.MainPortal.BaseTitleFa} | {settings.MainPortal.BaseTitleEn}",
                FeedImageContentPath = "~/favicon.png",
                FeedImageTitle = portalInfo.TitleFa,
                RssItems = rssItems,
                CultureName = "fa-IR"
            };

            return new FeedResult(channel);
        }
    }
}

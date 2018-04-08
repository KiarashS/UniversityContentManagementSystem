using ContentManagement.Common.WebToolkit;
using ContentManagement.Infrastructure.Seo;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;

namespace ContentManagement.Infrastructure.Seo
{
    public class SeoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;

        public SeoService(IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteSettings = siteSettings;
            _requestService = requestService;

            BaseLinkCanonical = _siteSettings.Value.BaseLinkCanonical;
            BaseTitle = _siteSettings.Value.BaseTitle;
            MetaDescription = _siteSettings.Value.BaseDescription;
            MetaContentLanguage = _requestService.CurrentLanguage().Language.ToString().ToLowerInvariant();
        }

        public string BaseLinkCanonical { get; set; }

        public string BaseTitle { get; set; }

        public string LinkCanonical { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaContentLanguage { get; set; }

        public string MetaAuthor { get; set; }

        public string MetaItemPropName { get; set; }

        public string MetaItemPropDescription { get; set; }

        public DateTimeOffset? MetaLastModified { get; set; }

        public RobotsIndex? MetaRobotsIndex { get; set; }

        public string Title { get; set; }

        public string TitleFormat { get; set; } = "{0} - {1}";

        public bool MetaRobotsNoIndex
        {
            get
            {
                return MetaRobotsIndex == RobotsIndexManager.DefaultRobotsNoIndex;
            }
            set
            {
                MetaRobotsIndex = value ? RobotsIndexManager.DefaultRobotsNoIndex : (RobotsIndex?)null;
            }
        }


        public string AddMetaKeyword(string addedKeyword)
        {
            if (addedKeyword == null)
            {
                throw new ArgumentNullException(nameof(addedKeyword));
            }

            string combinedMetaKeywords = CombineTexts(" ", this.MetaKeywords, addedKeyword);

            this.MetaKeywords = combinedMetaKeywords;

            return combinedMetaKeywords;
        }

        private static string CombineTexts(string separator, params string[] values)
        {
            var cleanedValues =
                (values?.Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>())
                .ToList();

            if (!cleanedValues.Any())
            {
                return null;
            }

            string combined = string.Join(separator, cleanedValues).Trim();
            return combined;
        }
    }
}

using System;

namespace ContentManagement.Infrastructure.Seo
{
    public static class SeoHelperTitleHelper
    {
        private static string DefaultTitleFormat = "{0} - {1}";

        public static string GetTitle(SeoService seoService, string value)
        {
            if (seoService == null)
            {
                throw new ArgumentNullException(nameof(seoService));
            }

            bool isBaseTitleSet = !string.IsNullOrWhiteSpace(seoService.BaseTitle);
            bool isPageTitleSet = !string.IsNullOrWhiteSpace(seoService.Title) || !string.IsNullOrWhiteSpace(value);

            if (isBaseTitleSet && isPageTitleSet)
            {
                string titleFormat = !string.IsNullOrWhiteSpace(seoService.TitleFormat)
                                         ? seoService.TitleFormat
                                         : DefaultTitleFormat;

                string title = string.Format(titleFormat, seoService.Title ?? value, seoService.BaseTitle);
                return title;
            }

            if (!isBaseTitleSet)
            {
                return (!string.IsNullOrWhiteSpace(seoService.Title) || !string.IsNullOrWhiteSpace(value)) ? seoService.Title ?? value : null;
            }

            return !string.IsNullOrWhiteSpace(seoService.BaseTitle) ? seoService.BaseTitle : null;
        }
    }
}

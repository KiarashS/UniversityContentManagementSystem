using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Common.WebToolkit
{
    public static class Seo
    {
        public static string GenerateMetaTag(string title, string description, bool allowIndexPage, bool allowFollowLinks, string author = "", string lastmodified = "", string expires = "never", string language = "fa", string faviconPath = "~/favicon.png", int maxLenghtTitle = 60, int maxLenghtDescription = 170)//, CacheControlType cacheControlType = CacheControlType._private)
        {
            title = title.Substring(0, title.Length <= maxLenghtTitle ? title.Length : maxLenghtTitle).Trim();
            description = description.Substring(0, description.Length <= maxLenghtDescription ? description.Length : maxLenghtDescription).Trim();

            var meta = "";
            meta += string.Format("<title>{0}</title>\n", title);
            meta += string.Format("<link rel=\"shortcut icon\" href=\"{0}\"/>\n", faviconPath);
            meta += string.Format("<meta http-equiv=\"content-language\" content=\"{0}\"/>\n", language);
            meta += string.Format("<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>\n");
            meta += string.Format("<meta charset=\"utf-8\"/>\n");
            meta += string.Format("<meta name=\"description\" content=\"{0}\"/>\n", description);
            //meta += string.Format("<meta http-equiv=\"Cache-control\" content=\"{0}\"/>\n", EnumExtensions.EnumHelper<CacheControlType>.GetEnumDescription(cacheControlType.ToString()));
            meta += string.Format("<meta name=\"robots\" content=\"{0}, {1}\" />\n", allowIndexPage ? "index" : "noindex", allowFollowLinks ? "follow" : "nofollow");
            meta += string.Format("<meta name=\"expires\" content=\"{0}\"/>\n", expires);
            meta += "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"/>\n";

            if (!string.IsNullOrEmpty(lastmodified))
                meta += string.Format("<meta name=\"last-modified\" content=\"{0}\"/>\n", lastmodified);

            if (!string.IsNullOrEmpty(author))
                meta += string.Format("<meta name=\"author\" content=\"{0}\"/>\n", author);

            //------------------------------------Google & Bing Doesn't Use Meta Keywords ...
            //meta += string.Format("<meta name=\"keywords\" content=\"{0}\"/>\n", keywords);

            return meta;
        }
    }

    //public enum CacheControlType
    //{
    //    [Description("public")]
    //    _public,
    //    [Description("private")]
    //    _private,
    //    [Description("no-cache")]
    //    _nocache,
    //    [Description("no-store")]
    //    _nostore
    //}
}

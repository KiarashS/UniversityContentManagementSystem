using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ContentManagement.Common.WebToolkit
{
    public static class HtmlExtensions
    {
        private static readonly Regex _pbrRegex = new Regex(@"<(?!br|/br|p|/p).+?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        private static readonly Regex _contentRegex = new Regex(@"<\/?script[^>]*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _safeStrRegex = new Regex(@"<script[^>]*?>[\s\S]*?<\/script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string NofollowExternalLinks(this string html, string baseDomain, string externalClassName = "external")
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var links = doc.DocumentNode.SelectNodes(@"//a[@href]");
            if(links == null)
            {
                return html;
            }

            foreach (var link in links)
            {
                var att = link.Attributes["href"];
                if (att == null) continue;
                var href = att.Value;
                if (href.StartsWith("javascript", StringComparison.InvariantCultureIgnoreCase) || href.StartsWith("#", StringComparison.InvariantCultureIgnoreCase)) continue;

                var urlNext = new Uri(href, UriKind.RelativeOrAbsolute);

                // Make it absolute if it's relative
                if (urlNext.IsAbsoluteUri && (!urlNext.Host.Contains(baseDomain)))
                {
                    // Absolute so it's external

                    link.Attributes.Append("rel", "nofollow");

                    link.Attributes.Remove("target");
                    link.Attributes.Add("target", "_blank");

                    link.Attributes.Append("class", externalClassName);
                }
            }

            return doc.DocumentNode.WriteTo();
        }

        public static string CleanAllTagsExceptContent(this string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode.InnerText.Replace(@"[\s\r\n\t]+", " ").Replace("&nbsp;", " ");
        }

        public static bool IsValidRequiredHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return false;
            }

            var content = html.CleanTags().Replace("&nbsp;", string.Empty).Trim();
            return !string.IsNullOrEmpty(content) && content.Length > 0;
        }

        /// <summary>
        /// حذف تمامی تگ‌ها منهای دو تگ ذکر شده
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string CleanTagsExceptPbr(this string html)
        {
            return _pbrRegex.Replace(html, string.Empty);
        }

        /// <summary>
        /// حذف تمامی تگ‌های موجود
        /// </summary>
        /// <param name="html">ورودی اچ تی ام ال</param>
        /// <returns></returns>
        public static string CleanTags(this string html)
        {
            return _htmlRegex.Replace(html, string.Empty);
        }

        /// <summary>
        /// حذف یک تگ ویژه بدون حذف محتویات آن
        /// </summary>
        /// <param name="html">ورودی اچ تی ام ال</param>
        /// <returns></returns>
        public static string CleanScriptTags(this string html)
        {
            return _contentRegex.Replace(html, string.Empty);
        }

        /// <summary>
        /// حذف یک تگ ویژه به همراه محتویات آن
        /// </summary>
        /// <param name="html">ورودی اچ تی ام ال</param>
        /// <returns></returns>
        public static string CleanScriptsTagsAndContents(this string html)
        {
            return _safeStrRegex.Replace(html, "");
        }
    }
}

using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    public class GoogleAnalyticsTagHelper : TagHelper
    {
        public string TrackingId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (!String.IsNullOrEmpty(TrackingId))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"<script async src=\"https://www.googletagmanager.com/gtag/js?id={TrackingId}\"></script>");
                sb.AppendLine("<script>");
                sb.AppendLine("  window.dataLayer = window.dataLayer || [];");
                sb.AppendLine("  function gtag(){dataLayer.push(arguments);}");
                sb.AppendLine("  gtag('js', new Date());");
                sb.AppendLine($"  gtag('config', '{TrackingId}');");
                sb.AppendLine("</script>");

                output.Content.SetHtmlContent(sb.ToString());
            }
        }
    }
}

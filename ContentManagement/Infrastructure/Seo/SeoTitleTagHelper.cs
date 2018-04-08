using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure.Seo
{
    [HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SeoTitleTagHelper : SeoTagHelperBase
    {
        internal const string TagName = "seo-title";

        internal const string ValueAttributeName = "value";

        [HtmlAttributeName(ValueAttributeName)]
        public string Value { get; set; }

        public SeoTitleTagHelper(SeoService seoService)
            : base(seoService)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var title = !string.IsNullOrWhiteSpace(Value) ? Value : _seoService.Title;

            title = SeoHelperTitleHelper.GetTitle(_seoService, Value) ?? title;

            if (title == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "title";

            output.Attributes.RemoveAll(nameof(Value));

            output.Content.SetContent(title);
        }
    }
}

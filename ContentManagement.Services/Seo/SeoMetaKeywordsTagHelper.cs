using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Services.Seo
{
    [HtmlTargetElement(TagName, TagStructure = TagStructure.WithoutEndTag)]
    public class SeoMetaKeywordsTagHelper : SeoTagHelperBase
    {
        internal const string TagName = "seo-meta-keywords";

        internal const string ValueAttributeName = "value";

        [HtmlAttributeName(ValueAttributeName)]
        public string Value { get; set; }

        public SeoMetaKeywordsTagHelper(SeoService seoService)
            : base(seoService)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var metaKeywords = !string.IsNullOrWhiteSpace(Value) ? Value : _seoService.MetaKeywords;
            if (metaKeywords == null)
            {
                output.SuppressOutput();
                return;
            }

            output.Attributes.RemoveAll(nameof(Value));

            this.SetMetaTagOutput(output, name: "keywords", content: metaKeywords);
        }
    }
}

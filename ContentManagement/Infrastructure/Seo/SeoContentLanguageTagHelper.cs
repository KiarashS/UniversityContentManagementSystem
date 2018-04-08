using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure.Seo
{
    [HtmlTargetElement(TagName, TagStructure = TagStructure.WithoutEndTag)]
    public class SeoContentLanguageTagHelper : SeoTagHelperBase
    {
        internal const string TagName = "seo-content-language";

        internal const string ValueAttributeName = "value";

        [HtmlAttributeName(ValueAttributeName)]
        public string Value { get; set; }

        public SeoContentLanguageTagHelper(SeoService seoService)
            : base(seoService)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var contentLanguage = !string.IsNullOrWhiteSpace(Value) ? Value : _seoService.MetaContentLanguage;
            if (contentLanguage == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "meta";

            output.Attributes.RemoveAll(nameof(Value));

            output.Attributes.Add(new TagHelperAttribute("http-equiv", "Content-Language", HtmlAttributeValueStyle.DoubleQuotes));
            output.Attributes.Add(new TagHelperAttribute("content", contentLanguage, HtmlAttributeValueStyle.DoubleQuotes));
        }
    }
}

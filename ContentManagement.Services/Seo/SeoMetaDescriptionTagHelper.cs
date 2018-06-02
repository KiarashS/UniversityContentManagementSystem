using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Services.Seo
{
    [HtmlTargetElement(TagName, TagStructure = TagStructure.WithoutEndTag)]
    public class SeoMetaDescriptionTagHelper : SeoTagHelperBase
    {
        internal const string TagName = "seo-meta-description";

        internal const string ValueAttributeName = "value";

        [HtmlAttributeName(ValueAttributeName)]
        public string Value { get; set; }

        public SeoMetaDescriptionTagHelper(SeoService seoService)
            : base(seoService)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var metaDescription = !string.IsNullOrWhiteSpace(Value) ? Value : _seoService.MetaDescription;
            if (metaDescription == null)
            {
                output.SuppressOutput();
                return;
            }

            output.Attributes.RemoveAll(nameof(Value));

            SetMetaTagOutput(output, name: "description", content: metaDescription);
        }
    }
}

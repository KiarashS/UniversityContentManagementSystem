using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Services.Seo
{
    [HtmlTargetElement(TagName, TagStructure = TagStructure.WithoutEndTag)]
    public class SeoMetaRobotsIndexTagHelper : SeoTagHelperBase
    {
        internal const string NoIndexAttributeName = "no-index";

        internal const string TagName = "seo-meta-robots-index";

        internal const string ValueAttributeName = "value";

        [HtmlAttributeName(NoIndexAttributeName)]
        public bool NoIndex { get; set; }

        [HtmlAttributeName(ValueAttributeName)]
        public RobotsIndex? Value { get; set; }

        public SeoMetaRobotsIndexTagHelper(SeoService seoService)
            : base(seoService)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var metaRobotsIndex = _seoService.MetaRobotsIndex ?? Value;

            metaRobotsIndex = NoIndex ? RobotsIndexManager.DefaultRobotsNoIndex : metaRobotsIndex;

            if (metaRobotsIndex == null)
            {
                output.SuppressOutput();
                return;
            }

            string content = RobotsIndexManager.GetMetaContent(metaRobotsIndex.Value);

            output.Attributes.RemoveAll(nameof(Value));
            output.Attributes.RemoveAll(nameof(NoIndex));

            this.SetMetaTagOutput(output, name: "robots", content: content);
        }
    }
}

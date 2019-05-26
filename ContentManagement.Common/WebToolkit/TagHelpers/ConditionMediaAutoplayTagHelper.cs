using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = AutoplayPrefix)]
    public class ConditionMediaAutoplayTagHelper : TagHelper
    {
        private const string AutoplayPrefix = "condition-autoplay";

        [HtmlAttributeName(AutoplayPrefix)]
        public string AutoplayAtr { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(AutoplayAtr) && bool.Parse(AutoplayAtr))
            {
                var attr = new TagHelperAttribute("autoplay", null, HtmlAttributeValueStyle.Minimized);
                output.Attributes.Add(attr);
            }
        }
    }
}

using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = ControlsPrefix)]
    public class ConditionMediaControlsTagHelper : TagHelper
    {
        private const string ControlsPrefix = "condition-controls";

        [HtmlAttributeName(ControlsPrefix)]
        public string ControlsAtr { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(ControlsAtr) && bool.Parse(ControlsAtr))
            {
                var attr = new TagHelperAttribute("controls", null, HtmlAttributeValueStyle.Minimized);
                output.Attributes.Add(attr);
            }
        }
    }
}

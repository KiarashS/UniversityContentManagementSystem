using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = WidthPrefix)]
    public class ConditionWidthTagHelper : TagHelper
    {
        private const string WidthPrefix = "condition-width";

        [HtmlAttributeName(WidthPrefix)]
        public string WidthAtr { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(WidthAtr))
            {
                output.Attributes.Add("width", WidthAtr);
            }
        }
    }
}

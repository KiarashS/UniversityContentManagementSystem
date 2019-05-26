using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = HeightPrefix)]
    public class ConditionHeightTagHelper : TagHelper
    {
        private const string HeightPrefix = "condition-height";

        [HtmlAttributeName(HeightPrefix)]
        public string HeightAtr { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(HeightAtr))
            {
                output.Attributes.Add("height", HeightAtr);
            }
        }
    }
}

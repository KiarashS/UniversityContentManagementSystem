using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = StylePrefix + "*")]
    public class ConditionStyleTagHelper : TagHelper
    {
        private const string StylePrefix = "condition-style-";

        [HtmlAttributeName("style")]
        public string StyleAtr { get; set; }

        private IDictionary<string, string> _styleValues;

        [HtmlAttributeName("", DictionaryAttributePrefix = StylePrefix)]
        public IDictionary<string, string> StyleValues
        {
            get
            {
                return _styleValues ?? (_styleValues =
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
            set { _styleValues = value; }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var items = _styleValues.Where(e => !string.IsNullOrEmpty(e.Value) && e.Value.Trim().Length > 0).Select(e => e.Key + ": " + e.Value).ToList();

            if (!string.IsNullOrEmpty(StyleAtr))
            {
                items.Insert(0, StyleAtr);
            }

            if (items.Any())
            {
                var styles = string.Join("; ", items.ToArray());
                output.Attributes.Add("style", styles);
            }
        }
    }
}

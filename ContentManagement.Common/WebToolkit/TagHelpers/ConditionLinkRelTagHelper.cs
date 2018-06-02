using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement(Attributes = RelPrefix + "*")]
    public class ConditionLinkRelTagHelper : TagHelper
    {
        private const string RelPrefix = "condition-rel-";

        [HtmlAttributeName("rel")]
        public string LinkRel { get; set; }

        private IDictionary<string, bool> _relValues;

        [HtmlAttributeName("", DictionaryAttributePrefix = RelPrefix)]
        public IDictionary<string, bool> ClassValues
        {
            get
            {
                return _relValues ?? (_relValues =
                    new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase));
            }
            set { _relValues = value; }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var items = _relValues.Where(e => e.Value).Select(e => e.Key).ToList();

            if (!string.IsNullOrEmpty(LinkRel))
            {
                items.Insert(0, LinkRel);
            }

            if (items.Any())
            {
                var rels = string.Join(" ", items.ToArray());
                output.Attributes.Add("rel", rels);
            }
        }
    }
}

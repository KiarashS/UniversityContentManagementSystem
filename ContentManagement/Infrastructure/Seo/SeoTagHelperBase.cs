using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure.Seo
{
    public abstract class SeoTagHelperBase : TagHelper
    {
        protected readonly SeoService _seoService;

        protected SeoTagHelperBase(SeoService seoService)
        {
            _seoService = seoService;
        }

        protected void SetMetaTagOutput(TagHelperOutput output, string name, string content)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (content == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "meta";

            output.Attributes.SetAttribute("name", name);
            output.Attributes.SetAttribute("content", content);
        }
    }
}

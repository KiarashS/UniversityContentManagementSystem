using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ContentManagement.Common.WebToolkit.TagHelpers
{
    [HtmlTargetElement("container")]
    public class ContainerTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Reset the TagName. We don't want `container` to render.
            output.TagName = null;
        }
    }
}

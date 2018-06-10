using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContentTypeTextEnAttribute : DescriptionAttribute
    {
        public ContentTypeTextEnAttribute(string description) : base(description)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContentTypeTextFaAttribute : DescriptionAttribute
    {
        public ContentTypeTextFaAttribute(string description) : base(description)
        {
        }
    }
}

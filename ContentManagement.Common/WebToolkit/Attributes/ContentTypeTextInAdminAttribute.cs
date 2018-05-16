using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContentTypeTextInAdminAttribute : DescriptionAttribute
    {
        public ContentTypeTextInAdminAttribute(string description) : base(description)
        {
        }
    }
}

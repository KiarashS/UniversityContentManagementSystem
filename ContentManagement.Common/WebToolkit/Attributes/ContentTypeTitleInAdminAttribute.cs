using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContentTypeTitleInAdminAttribute : DescriptionAttribute
    {
        public ContentTypeTitleInAdminAttribute(string description) : base(description)
        {
        }
    }
}

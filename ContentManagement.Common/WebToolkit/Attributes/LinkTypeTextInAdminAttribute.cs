using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class LinkTypeTextInAdminAttribute : DescriptionAttribute
    {
        public LinkTypeTextInAdminAttribute(string description) : base(description)
        {
        }
    }
}

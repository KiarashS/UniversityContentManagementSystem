using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContentGalleryPossitionTitleInAdminAttribute : DescriptionAttribute
    {
        public ContentGalleryPossitionTitleInAdminAttribute(string description) : base(description)
        {
        }
    }
}

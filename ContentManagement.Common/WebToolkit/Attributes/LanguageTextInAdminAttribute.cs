using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class LanguageTextInAdminAttribute : DescriptionAttribute
    {
        public LanguageTextInAdminAttribute(string description) : base(description)
        {
        }
    }
}

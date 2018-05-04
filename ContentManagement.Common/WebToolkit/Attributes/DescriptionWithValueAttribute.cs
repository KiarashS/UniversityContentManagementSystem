using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.Common.WebToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionWithValueAttribute : DescriptionAttribute
    {
        public DescriptionWithValueAttribute(string description, string value) : base(description)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}

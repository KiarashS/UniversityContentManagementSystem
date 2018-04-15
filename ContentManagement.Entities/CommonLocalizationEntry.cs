using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public abstract class CommonLocalizationEntry
    {
        public Language Language { get; set; } = Language.FA;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class FooterSection : IEntityType
    {
        public long                            Id               { get; set; }
        public int                             PortalId         { get; set; }
        public Language                        Language         { get; set; } = Language.FA;
        public string                          Title            { get; set; }
        public string                          Url              { get; set; }
        public bool                            IsEnable         { get; set; } = true;
        public int?                            Priority         { get; set; }
        public virtual Portal                  Portal           { get; set; }
        public virtual ICollection<FooterLink> Links            { get; set; }
    }
}

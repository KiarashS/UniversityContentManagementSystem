using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class Link : IEntityType
    {
        public long          Id               { get; set; }
        public int           PortalId         { get; set; }
        public Language      Language         { get; set; } = Language.FA;
        public LinkType      LinkType         { get; set; }
        public string        Text             { get; set; }
        public string        Icon             { get; set; }
        public string        IconColor        { get; set; }
        public string        Url              { get; set; }
        public bool          IsBlankUrlTarget { get; set; }
        public int?          Priority         { get; set; }
        public virtual       Portal Portal    { get; set; }
    }
}

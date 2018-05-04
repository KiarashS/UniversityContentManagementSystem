using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class Navbar : IEntityType
    {
        public Navbar()
        {
            Childrens = new HashSet<Navbar>();
        }

        public long          Id                            { get; set; }
        public long?         ParentId                      { get; set; }
        public int           PortalId                      { get; set; }
        public Language      Language                      { get; set; } = Language.FA;
        public string        Text                          { get; set; }
        public string        Url                           { get; set; }
        public bool          IsBlankUrlTarget              { get; set; }
        public int?          Priority                      { get; set; }
        public string        Icon                          { get; set; }
        public virtual       Navbar Parent                 { get; set; }
        public virtual       ICollection<Navbar> Childrens { get; set; }
        public virtual       Portal Portal                 { get; set; }
    }
}

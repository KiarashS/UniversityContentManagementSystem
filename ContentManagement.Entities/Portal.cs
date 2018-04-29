using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class Portal : IEntityType
    {
        public Portal()
        {
            Navbars = new HashSet<Navbar>();
            Pages = new HashSet<Page>();
            Slides = new HashSet<Slide>();
            Links = new HashSet<Link>();
            Contents = new HashSet<Content>();
        }

        public int     Id                            { get; set; }
        public string  PortalKey                     { get; set; } // Sub-Domain is considered as portal key, Portal with Null portal key is main portal.
        public string  TitleFa                       { get; set; }
        public string  DescriptionFa                 { get; set; }
        public string  LogoFilenameFa                { get; set; }
        public string  BulletinFa                    { get; set; }
        public string  TitleEn                       { get; set; }
        public string  DescriptionEn                 { get; set; }
        public string  LogoFilenameEn                { get; set; }
        public string  BulletinEn                    { get; set; }
        public virtual ICollection<Navbar> Navbars   { get; set; }
        public virtual ICollection<Page> Pages       { get; set; }
        public virtual ICollection<Slide> Slides     { get; set; }
        public virtual ICollection<Link> Links       { get; set; }
        public virtual ICollection<Content> Contents { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class FooterLink : IEntityType
    {
        public long          Id                          { get; set; }
        public long          FooterSectionId             { get; set; }
        public string        Text                        { get; set; }
        public string        Url                         { get; set; }
        public bool          IsBlankUrlTarget            { get; set; }
        public int?          Priority                    { get; set; }
        public virtual       FooterSection FooterSection { get; set; }
    }
}

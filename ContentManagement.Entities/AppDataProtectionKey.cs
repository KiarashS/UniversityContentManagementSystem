using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class AppDataProtectionKey// : IEntityType
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string XmlData { get; set; }
    }
}

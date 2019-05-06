using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class Slide : IEntityType
    {
        public long Id { get; set; }
        public int PortalId { get; set; }
        public Language Language { get; set; } = Language.FA;
        public string Title { get; set; } // image alt (*required)
        public string SupTitle { get; set; } // super title
        public string SubTitle { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public string Url { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public string Filename { get; set; }
        public int? Priority { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        public virtual Portal Portal { get; set; }
    }
}

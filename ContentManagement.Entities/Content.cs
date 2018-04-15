using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class Content : IEntityType
    {
        public long Id { get; set; }
        public int PortalId { get; set; }
        public Language Language { get; set; } = Language.FA;
        public ContentType ContentType { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageName { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFavorite { get; set; } = false;
        public virtual Portal Portal { get; set; }
    }
}

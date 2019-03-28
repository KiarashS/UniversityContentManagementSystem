using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ContentManagement.Entities
{
    public class Vote : IEntityType
    {
        public Vote()
        {
            Items = new HashSet<VoteItem>();
            VoteResults = new HashSet<VoteResult>();
        }

        public long Id { get; set; }
        public int PortalId { get; set; }
        public Language Language { get; set; } = Language.FA;
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsMultiChoice { get; set; }
        public bool IsVisibleResults { get; set; } = true;
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ExpireDate { get; set; }
        public virtual Portal Portal { get; set; }
        public virtual ICollection<VoteItem> Items { get; set; }
        public virtual ICollection<VoteResult> VoteResults { get; set; }
    }
}

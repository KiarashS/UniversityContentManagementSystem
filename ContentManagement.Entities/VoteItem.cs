using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class VoteItem : IEntityType
    {
        public VoteItem()
        {
            VoteItemResults = new HashSet<VoteResult>();
        }

        public long Id { get; set; }
        public long VoteId { get; set; }
        public string ItemTitle { get; set; }
        public int? Priority { get; set; }
        public virtual Vote Vote { get; set; }
        public virtual ICollection<VoteResult> VoteItemResults { get; set; }
    }
}

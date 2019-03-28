using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.Entities
{
    public class VoteResult : IEntityType
    {
        public long Id                   { get; set; }
        public long VoteId               { get; set; }
        public long VoteItemId           { get; set; }
        public virtual Vote Vote         { get; set; }
        public virtual VoteItem VoteItem { get; set; }
    }
}

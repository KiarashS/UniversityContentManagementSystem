using System;

namespace ContentManagement.Entities
{
    public class ActivityLog : IEntityType
    {
        public ActivityLog()
        {
            ActionDate = DateTimeOffset.UtcNow;
            ActionLevel = ActionLevel.Medium;
        }

        public long Id { get; set; }
        //public int UserId { get; set; }
        public string SourceAddress { get; set; } // IP Address
        public string ActionBy { get; set; } // Username
        public string ActionType { get; set; } // Add, Update, Delete, LogIn ...
        public string Message { get; set; }
        public string Portal { get; set; }
        public string Language { get; set; }
        public ActionLevel ActionLevel { get; set; }
        public DateTimeOffset ActionDate { get; set; }
        //public string OriginalValues { get; set; }
        //public string NewValues { get; set; }
        public string Url { get; set; }
        //public bool IsActionSucceeded { get; set; }
    }
}

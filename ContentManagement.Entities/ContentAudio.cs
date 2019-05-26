using System;

namespace ContentManagement.Entities
{
    public class ContentAudio : IEntityType
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Caption { get; set; }
        public string Audioname { get; set; }
        public bool EnableControls { get; set; } = true;
        public bool EnableAutoplay { get; set; }
        public int? Priority { get; set; }
        public virtual Content Content { get; set; }
    }
}

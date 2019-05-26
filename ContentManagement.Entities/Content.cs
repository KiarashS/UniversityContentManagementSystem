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
        public string RawText { get; set; }
        public string Summary { get; set; }
        public string Keywords { get; set; }
        public string Imagename { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ArchiveDate { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFavorite { get; set; } = false;
        public bool IsArchive { get; set; } = false;
        public int? Priority { get; set; }

        public ContentGalleryPosition GalleryPosition { get; set; } = ContentGalleryPosition.None;
        public ContentVideoPosition VideoPosition { get; set; } = ContentVideoPosition.None;
        public ContentAudioPosition AudioPosition { get; set; } = ContentAudioPosition.None;

        public virtual Portal Portal { get; set; }
        public virtual ICollection<ContentGallery> ContentGalleries { get; set; }
        public virtual ICollection<ContentVideo> ContentVideos { get; set; }
        public virtual ICollection<ContentAudio> ContentAudios { get; set; }
    }
}

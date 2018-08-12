using System;

namespace ContentManagement.Entities
{
    public class ContentGallery : IEntityType
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Caption { get; set; }
        public string Imagename { get; set; }
        public int? Priority { get; set; }
        //public ContentGalleryMediaType MediaType { get; set; } = ContentGalleryMediaType.Image;
        public virtual Content Content { get; set; }
    }
}

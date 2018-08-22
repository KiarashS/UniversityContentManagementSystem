using System;

namespace ContentManagement.Entities
{
    public class Page : IEntityType
    {
        public long           Id            { get; set; }
        public int            PortalId      { get; set; }
        public Language       Language      { get; set; } = Language.FA;
        public string         Title         { get; set; }
        public string         Text          { get; set; }
        public string         RawText       { get; set; }
        public string         Slug          { get; set; }
        public string         Keywords      { get; set; }
        public string         Imagename     { get; set; }
        public DateTimeOffset PublishDate   { get; set; } = DateTimeOffset.UtcNow;
        public int            ViewCount     { get; set; }
        public bool           IsActive      { get; set; } = true;
        public virtual        Portal Portal { get; set; }
    }
}

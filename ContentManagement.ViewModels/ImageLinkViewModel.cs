using ContentManagement.Entities;

namespace ContentManagement.ViewModels
{
    public class ImageLinkViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Imagename { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}

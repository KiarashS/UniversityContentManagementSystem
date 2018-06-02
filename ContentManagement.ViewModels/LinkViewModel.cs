using ContentManagement.Entities;

namespace ContentManagement.ViewModels
{
    public class LinkViewModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public LinkType LinkType { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}

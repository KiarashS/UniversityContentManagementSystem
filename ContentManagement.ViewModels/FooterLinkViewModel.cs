using System.ComponentModel.DataAnnotations;

namespace ContentManagement.ViewModels
{
    public class FooterLinkViewModel
    {
        public long Id { get; set; }
        public long FooterSectionId { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}

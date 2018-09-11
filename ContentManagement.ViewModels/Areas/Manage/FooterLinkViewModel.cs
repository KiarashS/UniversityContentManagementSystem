using System.ComponentModel.DataAnnotations;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class FooterLinkViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "متن لینک را وارد نمایید.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "آدرس لینک را وارد نمایید.")]
        public string Url { get; set; }
        //public string Icon { get; set; }
        //public string IconColor { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}

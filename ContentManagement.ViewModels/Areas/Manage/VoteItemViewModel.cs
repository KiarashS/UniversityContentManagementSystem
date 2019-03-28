using System.ComponentModel.DataAnnotations;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class VoteItemViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "عنوان گزینه را وارد نمایید.")]
        public string ItemTitle { get; set; }
        public int? Priority { get; set; }
    }
}

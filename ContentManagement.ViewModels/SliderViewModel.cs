using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class SliderViewModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public string Url { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public string Filename { get; set; }
        public int? Priority { get; set; }
    }
}

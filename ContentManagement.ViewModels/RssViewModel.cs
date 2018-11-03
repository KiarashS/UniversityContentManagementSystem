using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class RssViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public IList<string> Categories { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public Language Language { get; set; }
    }
}

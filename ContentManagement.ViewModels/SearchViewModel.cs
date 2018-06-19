using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            ContentsViewModel = new List<ContentsViewModel>();
        }

        public IList<ContentsViewModel> ContentsViewModel { get; set; }
        public int Page { get; set; }
        public long TotalCount { get; set; }
        public int Start { get; set; }
        public int PageSize { get; set; }
        public string Q { get; set; }
    }
}

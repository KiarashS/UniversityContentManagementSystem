using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class ContentGalleryViewModel
    {
        public long Id { get; set; }
        public string Caption { get; set; }
        public string Imagename { get; set; }
        public int? Priority { get; set; }
    }
}

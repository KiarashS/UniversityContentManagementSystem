using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure
{
    public static class Constants
    {
        public static readonly string SlidesRootPath = "slides";
        public static readonly string PagesRootPath = "pagesimage";
        public static readonly string ContentsRootPath = "contentimg";
        public static readonly string FilesManagerRootPath = "contentfiles";
        public static readonly string ImagesManagerRootPath = "contentimages";

        public static readonly int SlideWidthSize = 1110;
        public static readonly int SlideHeightSize = 350;

        public static readonly int PageImageWidthSize = 700;
        public static readonly int PageImageHeightSize = 250;

        public static readonly int ContentImageWidthSize = 700;
        public static readonly int ContentImageHeightSize = 250;

        public static readonly int EditorImageThumbWidthSize = 80;
        public static readonly int EditorImageThumbHeightSize = 80;
    }
}

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
        public static readonly string ImageLinksRootPath = "linkimages";
        public static readonly string FilesManagerRootPath = "contentfiles";
        public static readonly string ImagesManagerRootPath = "contentimages";

        public static readonly int SlideWidthSize = 1170;
        public static readonly int SlideHeightSize = 350;

        public static readonly int PageImageWidthSize = 820;
        public static readonly int PageImageHeightSize = 300;

        public static readonly int ContentImageWidthSize = 820;
        public static readonly int ContentImageHeightSize = 300;

        public static readonly int LinkImageWidthSize = 350;
        public static readonly int LinkImageHeightSize = 350;

        public static readonly int EditorImageThumbWidthSize = 80;
        public static readonly int EditorImageThumbHeightSize = 80;
    }
}

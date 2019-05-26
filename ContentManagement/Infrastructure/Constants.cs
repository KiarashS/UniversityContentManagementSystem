using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        public static readonly string ContentGalleriesRootPath = "contentgalleries";
        public static readonly string ContentVideosRootPath = "contentvideos";
        public static readonly string ContentAudiosRootPath = "contentaudios";

        public static readonly int SlideWidthSize = 1890;
        public static readonly int SlideHeightSize = 750;

        public static readonly int PageImageWidthSize = 820;
        public static readonly int PageImageHeightSize = 300;

        public static readonly int ContentImageWidthSize = 820;
        public static readonly int ContentImageHeightSize = 300;

        public static readonly int ContentGalleryWidthSize = 2000;
        public static readonly int ContentGalleryHeightSize = 2000;

        public static readonly int LinkImageWidthSize = 350;
        public static readonly int LinkImageHeightSize = 350;

        public static readonly int EditorImageThumbWidthSize = 80;
        public static readonly int EditorImageThumbHeightSize = 80;


        public static string GetFilesFolder(ClaimsPrincipal user)
        {
            var identity = (ClaimsIdentity)user.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).ToList();

            if (!roles.Any(x => x.Value.ToLowerInvariant() == "admin"))
            {
                var portalId = claims.SingleOrDefault(x => x.Type.ToLowerInvariant() == "portalid").Value;
                //var subPortalPath = Path.Combine("SubPortals", portalId);

                if (!string.IsNullOrEmpty(portalId))
                {
                    return $"{FilesManagerRootPath}/SubPortals/{portalId}";
                }
            }

            return FilesManagerRootPath;
        }

        public static string GetImagesFolder(ClaimsPrincipal user)
        {
            var identity = (ClaimsIdentity)user.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).ToList();

            if (!roles.Any(x => x.Value.ToLowerInvariant() == "admin"))
            {
                var portalId = claims.SingleOrDefault(x => x.Type.ToLowerInvariant() == "portalid").Value;
                //var subPortalPath = Path.Combine("SubPortals", portalId);

                if (!string.IsNullOrEmpty(portalId))
                {
                    return $"{ImagesManagerRootPath}/SubPortals/{portalId}";
                }
            }

            return ImagesManagerRootPath;
        }

    }
}

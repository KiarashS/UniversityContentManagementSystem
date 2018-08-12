using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IContentGalleryService
    {
        Task AddOrUpdateGalleryAsync(long contentId, ContentGalleryViewModel gallery);
        Task<ContentGallery> FindGalleryByIdAsync(long contentId, long galleryId);
        Task<IList<ContentGalleryViewModel>> GetPagedGalleriesAsync(long contentId);
        Task<long> GalleryCountAsync(long contentId);
        Task DeleteGalleryAsync(long contentId, long id);
        Task<string> GetGalleryImagenameAsync(long contentId, long id);
        Task<IList<ContentManagement.ViewModels.ContentGalleryViewModel>> GetContentGalleryAsync(long contentId);
    }
}

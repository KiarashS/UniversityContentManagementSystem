namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ContentGalleryService : IContentGalleryService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ContentGallery> _contentGalleries;

        public ContentGalleryService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _contentGalleries = _uow.Set<ContentGallery>();
        }

        public async Task AddOrUpdateGalleryAsync(long contentId, ContentGalleryViewModel gallery)
        {
            if (gallery.Id == 0) // Add
            {
                var newGallery = new ContentGallery
                {
                    Caption = gallery.Caption?.Trim(),
                    Priority = gallery.Priority,
                    ContentId = contentId,
                    Imagename = gallery.Imagename,
                };

                _contentGalleries.Add(newGallery);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            
            var currentGallery = await FindGalleryByIdAsync(contentId, gallery.Id).ConfigureAwait(false);

            currentGallery.Caption = gallery.Caption?.Trim();
            currentGallery.Priority = gallery.Priority;
            currentGallery.Imagename = gallery.Imagename;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteGalleryAsync(long contentId, long id)
        {
            var gallery = new ContentGallery { Id = id, ContentId = contentId };

            _uow.Entry(gallery).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ContentGallery> FindGalleryByIdAsync(long contentId, long galleryId)
        {
            var gallery = await _contentGalleries.FirstOrDefaultAsync(x => x.Id == galleryId && x.ContentId == contentId).ConfigureAwait(false);
            return gallery;
        }

        public async Task<IList<ContentGalleryViewModel>> GetPagedGalleriesAsync(long contentId)
        {
            var galleries = await _contentGalleries.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Cacheable()
                                    .ToListAsync();

            return galleries.Select(x => new ContentGalleryViewModel { Id = x.Id, Imagename = x.Imagename, ContentId = x.ContentId, Priority = x.Priority, Caption = x.Caption }).ToList();
        }

        public async Task<string> GetGalleryImagenameAsync(long contentId, long id)
        {
            var imagename = await _contentGalleries.Where(x => x.Id == id && x.ContentId == contentId).Select(x => x.Imagename).SingleOrDefaultAsync().ConfigureAwait(false);
            return imagename;
        }

        public async Task<long> GalleryCountAsync(long contentId)
        {
            var count = await _contentGalleries.LongCountAsync(x => x.ContentId == contentId).ConfigureAwait(false);
            return count;
        }

        public async Task<IList<ContentManagement.ViewModels.ContentGalleryViewModel>> GetContentGalleryAsync(long contentId)
        {
            var galleries = await _contentGalleries.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Select(x => new { x.Id, x.Imagename, x.Priority, x.Caption })
                                    .Cacheable()
                                    .ToListAsync();

            return galleries.Select(x => new ContentManagement.ViewModels.ContentGalleryViewModel { Id = x.Id, Imagename = x.Imagename, Priority = x.Priority, Caption = x.Caption }).ToList();
        }
    }
}

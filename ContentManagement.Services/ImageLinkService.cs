namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageLinkService : IImageLinkService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ImageLink> _imageLink;

        public ImageLinkService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _imageLink = _uow.Set<ImageLink>();
        }

        public async Task AddOrUpdateImageLinkAsync(ImageLinkViewModel imageLink)
        {
            if (imageLink.Id == 0) // Add
            {
                var newImageLink = new ImageLink
                {
                    Title = imageLink.Title?.Trim(),
                    Description = imageLink.Description?.Trim(),
                    Url = imageLink.Url?.Trim(),
                    Imagename = imageLink.Imagename,
                    IsBlankUrlTarget = imageLink.IsBlankUrlTarget,
                    Priority = imageLink.Priority,
                    Language = imageLink.Language,
                    PortalId = imageLink.PortalId
                };

                _imageLink.Add(newImageLink);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            var currentImageLink = await FindImageLinkByIdAsync(imageLink.Id).ConfigureAwait(false);

            currentImageLink.Title = imageLink.Title?.Trim();
            currentImageLink.Description = imageLink.Description?.Trim();
            currentImageLink.Url = imageLink.Url?.Trim();
            currentImageLink.Imagename = imageLink.Imagename;
            currentImageLink.IsBlankUrlTarget = imageLink.IsBlankUrlTarget;
            currentImageLink.Priority = imageLink.Priority;
            currentImageLink.Language = imageLink.Language;
            currentImageLink.PortalId = imageLink.PortalId;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteImageLinkAsync(long id)
        {
            var imageLink = new ImageLink { Id = id };

            _uow.Entry(imageLink).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ImageLink> FindImageLinkByIdAsync(long imagelinkId)
        {
            var imageLink = await _imageLink.FirstOrDefaultAsync(x => x.Id == imagelinkId).ConfigureAwait(false);
            return imageLink;
        }

        public async Task<IList<ImageLinkViewModel>> GetPagedImageLinksAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _imageLink.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var links = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.Description, x.Url, x.Imagename, x.IsBlankUrlTarget, x.Priority })
                                    .ToListAsync();

            return links.Select(x => new ImageLinkViewModel { Id = x.Id, Title = x.Title, Description = x.Description, Url = x.Url, Imagename = x.Imagename, IsBlankUrlTarget = x.IsBlankUrlTarget, Priority = x.Priority }).ToList();
        }

        public async Task<IList<ViewModels.ImageLinkViewModel>> GetImageLinksAsync(string portalKey, Language language, int maxSize = 4)
        {
            var imageLinks = await 
                _imageLink
                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language)
                .Select(x => new { x.Id, x.Url, x.Title, x.Description, x.Imagename, x.IsBlankUrlTarget, x.Priority })
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.Id)
                .Take(maxSize)
                .Cacheable()
                .ToListAsync();

            var imageLinkViewModel = new List<ViewModels.ImageLinkViewModel>();

            foreach (var item in imageLinks)
            {
                imageLinkViewModel.Add(new ViewModels.ImageLinkViewModel
                {
                    Id = item.Id,
                    Url = item.Url,
                    Title = item.Title,
                    Description = item.Description,
                    Imagename = item.Imagename,
                    IsBlankUrlTarget = item.IsBlankUrlTarget,
                    Priority = item.Priority
                });
            }

            return imageLinkViewModel;
        }

        public async Task<long> ImageLinksCountAsync()
        {
            var imageLink = await _imageLink.LongCountAsync().ConfigureAwait(false);
            return imageLink;
        }

        public async Task<long> ImageLinksPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null)
        {
            var query = _imageLink.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<string> GetImageLinkImagenameAsync(long id)
        {
            var imagename = await _imageLink.Where(x => x.Id == id).Select(x => x.Imagename).SingleOrDefaultAsync().ConfigureAwait(false);
            return imagename;
        }
    }
}

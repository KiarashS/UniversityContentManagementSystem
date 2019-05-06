namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SlideService : ISlideService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Slide> _slide;

        public SlideService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _slide = _uow.Set<Slide>();
        }

        public async Task AddOrUpdateSlideAsync(SlideViewModel slide)
        {
            if (slide.Id == 0) // Add
            {
                var newSlide = new Slide
                {
                    Title = slide.Title?.Trim(),
                    Url = slide.Url?.Trim(),
                    SupTitle = slide.SupTitle?.Trim(),
                    SubTitle = slide.SubTitle?.Trim(),
                    IsBlankUrlTarget = slide.IsBlankUrlTarget,
                    Priority = slide.Priority,
                    Filename = slide.Filename,
                    Language = slide.Language,
                    PortalId = slide.PortalId,
                    PublishDate = slide.PublishDate.ToUniversalTime(),
                    ExpireDate = slide.ExpireDate?.ToUniversalTime()
                };

                _slide.Add(newSlide);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentSlide = await FindSlideByIdAsync(slide.Id).ConfigureAwait(false);

            currentSlide.Title = slide.Title?.Trim();
            currentSlide.SupTitle = slide.SupTitle?.Trim();
            currentSlide.SubTitle = slide.SubTitle?.Trim();
            currentSlide.Url = slide.Url?.Trim();
            currentSlide.IsBlankUrlTarget = slide.IsBlankUrlTarget;
            currentSlide.Priority = slide.Priority;
            currentSlide.Filename = slide.Filename;
            currentSlide.Language = slide.Language;
            currentSlide.PortalId = slide.PortalId;
            currentSlide.PublishDate = slide.PublishDate.ToUniversalTime();
            currentSlide.ExpireDate = slide.ExpireDate?.ToUniversalTime();

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteSlideAsync(long id)
        {
            var slide = new Slide { Id = id };

            _uow.Entry(slide).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Slide> FindSlideByIdAsync(long slideId)
        {
            var slide = await _slide.FirstOrDefaultAsync(x => x.Id == slideId);
            return slide;
        }

        public async Task<IList<SlideViewModel>> GetPagedSlidesAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _slide.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.SupTitle.Contains(searchTerm) || x.SubTitle.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var slides = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.SupTitle, x.SubTitle, x.Url, x.Filename, x.PublishDate, x.ExpireDate, x.IsBlankUrlTarget, x.Priority })
                                    .ToListAsync();

            return slides.Select(x => new SlideViewModel { Id = x.Id, Title = x.Title, SupTitle = x.SupTitle, SubTitle = x.SubTitle, Filename = x.Filename, PublishDate = x.PublishDate, ExpireDate = x.ExpireDate, IsBlankUrlTarget = x.IsBlankUrlTarget, Url = x.Url, Priority = x.Priority }).ToList();
        }

        public async Task<string> GetSlideFilenameAsync(long id)
        {
            var filename = await _slide.Where(x => x.Id == id).Select(x => x.Filename).SingleOrDefaultAsync().ConfigureAwait(false);
            return filename;
        }

        public async Task<IList<SliderViewModel>> GetPortalSlidesAsync(string portalKey, Language language, int size)
        {
            var sliderViewModel = new List<SliderViewModel>();
            var nowDate = DateTimeOffset.UtcNow;
            var slides = await _slide
                                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.PublishDate <= nowDate && (x.ExpireDate== null || x.ExpireDate >= nowDate))
                                .OrderByDescending(x => x.Priority)
                                .ThenByDescending(x => x.Id)
                                .Select(x => new { x.Title, x.SupTitle, x.SubTitle, x.Url, x.PublishDate, x.Priority, x.IsBlankUrlTarget, x.Filename })
                                .Cacheable()
                                .ToListAsync();
            
            foreach (var item in slides)
            {
                sliderViewModel.Add(new SliderViewModel {
                    Url = item.Url,
                    Title = item.Title,
                    SupTitle = item.SupTitle,
                    SubTitle = item.SubTitle,
                    PublishDate = item.PublishDate,
                    Priority = item.Priority,
                    Filename = item.Filename,
                    IsBlankUrlTarget = item.IsBlankUrlTarget
                });
            }

            return sliderViewModel;
        }

        public async Task<long> SlideCountAsync()
        {
            var count = await _slide.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<long> SlidePagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null)
        {
            var query = _slide.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.SupTitle.Contains(searchTerm) || x.SubTitle.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }
    }
}

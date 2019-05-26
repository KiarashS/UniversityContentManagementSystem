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

    public class ContentVideoService : IContentVideoService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ContentVideo> _contentVideos;

        public ContentVideoService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _contentVideos = _uow.Set<ContentVideo>();
        }

        public async Task AddOrUpdateVideoAsync(long contentId, ContentVideoViewModel video)
        {
            if (video.Id == 0) // Add
            {
                var newVideo = new ContentVideo
                {
                    Caption = video.Caption?.Trim(),
                    Width = video.Width,
                    Height = video.Height,
                    EnableControls = video.EnableControls,
                    EnableAutoplay = video.EnableAutoplay,
                    Priority = video.Priority,
                    ContentId = contentId,
                    Videoname = video.Videoname,
                };

                _contentVideos.Add(newVideo);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            
            var currentVideo = await FindVideoByIdAsync(contentId, video.Id).ConfigureAwait(false);

            currentVideo.Caption = video.Caption?.Trim();
            currentVideo.Width = video.Width;
            currentVideo.Height = video.Height;
            currentVideo.EnableControls = video.EnableControls;
            currentVideo.EnableAutoplay = video.EnableAutoplay;
            currentVideo.Priority = video.Priority;
            currentVideo.Videoname = video.Videoname;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteVideoAsync(long contentId, long id)
        {
            var video = new ContentVideo { Id = id, ContentId = contentId };

            _uow.Entry(video).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ContentVideo> FindVideoByIdAsync(long contentId, long videoId)
        {
            var video = await _contentVideos.FirstOrDefaultAsync(x => x.Id == videoId && x.ContentId == contentId).ConfigureAwait(false);
            return video;
        }

        public async Task<IList<ContentVideoViewModel>> GetPagedVideosAsync(long contentId)
        {
            var videos = await _contentVideos.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Cacheable()
                                    .ToListAsync();

            return videos.Select(x => new ContentVideoViewModel { Id = x.Id, Videoname = x.Videoname, ContentId = x.ContentId, Priority = x.Priority, Caption = x.Caption, Width = x.Width, Height = x.Height, EnableControls = x.EnableControls, EnableAutoplay = x.EnableAutoplay }).ToList();
        }

        public async Task<string> GetVideonameAsync(long contentId, long id)
        {
            var videoname = await _contentVideos.Where(x => x.Id == id && x.ContentId == contentId).Select(x => x.Videoname).SingleOrDefaultAsync().ConfigureAwait(false);
            return videoname;
        }

        public async Task<long> ContentVideosCountAsync(long contentId)
        {
            var count = await _contentVideos.LongCountAsync(x => x.ContentId == contentId).ConfigureAwait(false);
            return count;
        }

        public async Task<IList<ViewModels.ContentVideoViewModel>> GetContentVideosAsync(long contentId)
        {
            var videos = await _contentVideos.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Select(x => new { x.Id, x.Videoname, x.Priority, x.Caption, x.Width, x.Height, x.EnableControls, x.EnableAutoplay })
                                    .Cacheable()
                                    .ToListAsync();

            return videos.Select(x => new ViewModels.ContentVideoViewModel { Id = x.Id, Videoname = x.Videoname, Priority = x.Priority, Caption = x.Caption, Width = x.Width, Height = x.Height, EnableControls = x.EnableControls, EnableAutoplay = x.EnableAutoplay }).ToList();
        }
    }
}

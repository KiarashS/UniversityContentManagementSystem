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

    public class ContentAudioService : IContentAudioService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ContentAudio> _contentAudios;

        public ContentAudioService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _contentAudios = _uow.Set<ContentAudio>();
        }

        public async Task AddOrUpdateAudioAsync(long contentId, ContentAudioViewModel audio)
        {
            if (audio.Id == 0) // Add
            {
                var newAudio = new ContentAudio
                {
                    Caption = audio.Caption?.Trim(),
                    EnableControls = audio.EnableControls,
                    EnableAutoplay = audio.EnableAutoplay,
                    Priority = audio.Priority,
                    ContentId = contentId,
                    Audioname = audio.Audioname,
                };

                _contentAudios.Add(newAudio);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            
            var currentAudio = await FindAudioByIdAsync(contentId, audio.Id).ConfigureAwait(false);

            currentAudio.Caption = audio.Caption?.Trim();
            currentAudio.EnableControls = audio.EnableControls;
            currentAudio.EnableAutoplay = audio.EnableAutoplay;
            currentAudio.Priority = audio.Priority;
            currentAudio.Audioname = audio.Audioname;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAudioAsync(long contentId, long id)
        {
            var audio = new ContentAudio { Id = id, ContentId = contentId };

            _uow.Entry(audio).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ContentAudio> FindAudioByIdAsync(long contentId, long audioId)
        {
            var audio = await _contentAudios.FirstOrDefaultAsync(x => x.Id == audioId && x.ContentId == contentId).ConfigureAwait(false);
            return audio;
        }

        public async Task<IList<ContentAudioViewModel>> GetPagedAudiosAsync(long contentId)
        {
            var audios = await _contentAudios.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Cacheable()
                                    .ToListAsync();

            return audios.Select(x => new ContentAudioViewModel { Id = x.Id, Audioname = x.Audioname, ContentId = x.ContentId, Priority = x.Priority, Caption = x.Caption, EnableControls = x.EnableControls, EnableAutoplay = x.EnableAutoplay }).ToList();
        }

        public async Task<string> GetAudionameAsync(long contentId, long id)
        {
            var audioname = await _contentAudios.Where(x => x.Id == id && x.ContentId == contentId).Select(x => x.Audioname).SingleOrDefaultAsync().ConfigureAwait(false);
            return audioname;
        }

        public async Task<long> ContentAudiosCountAsync(long contentId)
        {
            var count = await _contentAudios.LongCountAsync(x => x.ContentId == contentId).ConfigureAwait(false);
            return count;
        }

        public async Task<IList<ViewModels.ContentAudioViewModel>> GetContentAudiosAsync(long contentId)
        {
            var audios = await _contentAudios.Where(x => x.ContentId == contentId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Select(x => new { x.Id, x.Audioname, x.Priority, x.Caption, x.EnableControls, x.EnableAutoplay })
                                    .Cacheable()
                                    .ToListAsync();

            return audios.Select(x => new ViewModels.ContentAudioViewModel { Id = x.Id, Audioname = x.Audioname, Priority = x.Priority, Caption = x.Caption, EnableControls = x.EnableControls, EnableAutoplay = x.EnableAutoplay }).ToList();
        }
    }
}

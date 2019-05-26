using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IContentAudioService
    {
        Task AddOrUpdateAudioAsync(long contentId, ContentAudioViewModel audio);
        Task<ContentAudio> FindAudioByIdAsync(long contentId, long audioId);
        Task<IList<ContentAudioViewModel>> GetPagedAudiosAsync(long contentId);
        Task<long> ContentAudiosCountAsync(long contentId);
        Task DeleteAudioAsync(long contentId, long id);
        Task<string> GetAudionameAsync(long contentId, long id);
        Task<IList<ViewModels.ContentAudioViewModel>> GetContentAudiosAsync(long contentId);
    }
}

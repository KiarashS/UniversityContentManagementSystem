using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IContentVideoService
    {
        Task AddOrUpdateVideoAsync(long contentId, ContentVideoViewModel video);
        Task<ContentVideo> FindVideoByIdAsync(long contentId, long videoId);
        Task<IList<ContentVideoViewModel>> GetPagedVideosAsync(long contentId);
        Task<long> ContentVideosCountAsync(long contentId);
        Task DeleteVideoAsync(long contentId, long id);
        Task<string> GetVideonameAsync(long contentId, long id);
        Task<IList<ViewModels.ContentVideoViewModel>> GetContentVideosAsync(long contentId);
    }
}

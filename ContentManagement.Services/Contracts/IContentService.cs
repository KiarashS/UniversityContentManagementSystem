using ContentManagement.Entities;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IContentService
    {
        Task AddOrUpdateContentAsync(ContentViewModel content);
        Task<Content> FindContentByIdAsync(long contentId);
        Task<IList<ContentViewModel>> GetPagedContentsAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> ContentsCountAsync();
        Task<long> ContentsPagedCountAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null);
        Task DeleteContentAsync(long id);
        Task<string> GetContentImagenameAsync(long id);
        Task<IList<SubPortalsContentsViewModel>> GetSubPortalsContentsAsync(Language language = Language.FA, int maxSize = 30);
    }
}

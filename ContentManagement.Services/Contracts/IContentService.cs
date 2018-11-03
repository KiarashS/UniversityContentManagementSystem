using ContentManagement.Entities;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using DNTCommon.Web.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IContentService
    {
        Task AddOrUpdateContentAsync(ContentManagement.ViewModels.Areas.Manage.ContentViewModel content);
        Task<Content> FindContentByIdAsync(long contentId);
        Task<IList<ContentManagement.ViewModels.Areas.Manage.ContentViewModel>> GetPagedContentsAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> ContentsCountAsync();
        Task<long> ContentsPagedCountAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null);
        Task DeleteContentAsync(long id);
        Task<string> GetContentImagenameAsync(long id);
        Task<IList<SubPortalsContentsViewModel>> GetSubPortalsContentsAsync(Language language = Language.FA, int maxSize = 30);
        Task<IList<ContentsViewModel>> GetContentsAsync(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News, int start = 0, int length = 10);
        Task<IList<ContentsViewModel>> GetOtherContentsAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10);
        Task<IList<ContentsViewModel>> GetFavoritesAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10);
        Task<bool> IsExistContent(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News);
        Task<bool> IsExistFavorite(string portalKey, ContentType? contentType, Language language = Language.FA);
        Task<IList<ViewModels.ContentVisibilityViewModel>> CheckContentsVisibility(string portalKey, Language language);
        Task<ViewModels.ContentViewModel> GetContentDetails(string portalKey, Language language, long id);
        Task<long> OtherContentsCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA);
        Task<string> GetTitle(string portalKey, Language language, long id);
        Task<long> FavoritesCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA);
        Task<long> ContentsCountAsync(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News);
        Task<IList<SearchAutoCompleteViewModel>> GetSearchAutoCompleteAsync(string portalKey, Language language, string searchQuery, int size = 15);
        Task<IList<ContentsViewModel>> GetSearchResultsAsync(string portalKey, Language language, string searchQuery, int start = 0, int size = 15);
        Task<long> GetSearchResultsCountAsync(string portalKey, Language language, string searchQuery);
        Task<ContentGalleryPosition> GetGalleryPosition(long contentId);
        Task UpdateGalleryPosition(long contentId, ContentGalleryPosition newPosition);
        Task<bool> HasGallery(long contentId);
        Task<IList<RssViewModel>> GetRssResult(string portalKey, ContentType? contentType, int size = 20);
    }
}

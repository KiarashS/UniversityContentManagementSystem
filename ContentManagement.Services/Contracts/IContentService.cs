using ContentManagement.Entities;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using DNTCommon.Web.Core;
using System;
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
        Task<long> ContentsCountAsync(string portalKey);
        Task<long> ContentsPagedCountAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null);
        Task DeleteContentAsync(long id);
        Task<string> GetContentImagenameAsync(long id);
        Task<IList<SubPortalsContentsViewModel>> GetSubPortalsContentsAsync(Language language = Language.FA, int maxSize = 30);
        Task<IList<ContentsViewModel>> GetContentsAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10, bool isArchive = false);
        Task<IList<ContentsViewModel>> GetOtherContentsAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10, bool isArchive = false);
        Task<IList<ContentsViewModel>> GetFavoritesAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10, bool isArchive = false);
        Task<bool> IsExistContent(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News);
        Task<bool> IsExistFavorite(string portalKey, ContentType? contentType, Language language = Language.FA);
        Task<IList<ViewModels.ContentVisibilityViewModel>> CheckContentsVisibility(string portalKey, Language language);
        Task<ViewModels.ContentViewModel> GetContentDetails(string portalKey, Language language, long id);
        Task<long> OtherContentsCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA, bool isArchive = false);
        Task<string> GetTitle(string portalKey, Language language, long id);
        Task<long> FavoritesCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA, bool isArchive = false);
        Task<long> ContentsCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA, bool isArchive = false);
        Task<IList<SearchAutoCompleteViewModel>> GetSearchAutoCompleteAsync(string portalKey, Language language, string searchQuery, int size = 15);
        Task<IList<ContentsViewModel>> GetSearchResultsAsync(string portalKey, Language language, string searchQuery, int start = 0, int size = 15);
        Task<long> GetSearchResultsCountAsync(string portalKey, Language language, string searchQuery);
        Task<ContentGalleryPosition> GetGalleryPosition(long contentId);
        Task UpdateGalleryPosition(long contentId, ContentGalleryPosition newPosition);
        Task<ContentVideoPosition> GetVideosPosition(long contentId);
        Task UpdateVideoPosition(long contentId, ContentVideoPosition newPosition);
        Task<ContentAudioPosition> GetAudiosPosition(long contentId);
        Task UpdateAudioPosition(long contentId, ContentAudioPosition newPosition);
        Task<bool> HasGallery(long contentId);
        Task<bool> HasVideo(long contentId);
        Task<bool> HasAudio(long contentId);
        Task<IList<RssViewModel>> GetRssResult(string portalKey, ContentType? contentType, int size = 20);
        Task<bool> IsExistContent(string portalKey, Language language = Language.FA);
        Task<IList<ContentsViewModel>> GetMostViewedContentsAsync(string portalKey, Language language = Language.FA, int size = 10);
        Task<PdfViewModel> GetDataForPdfAsyc(long id, string portalKey, Language language = Language.FA);
        Task<bool> IsExistArchiveAsync(long id, string portalKey, Language language = Language.FA);
        Task<bool> IsExistArchiveAsync(string portalKey, Language language = Language.FA);
    }
}

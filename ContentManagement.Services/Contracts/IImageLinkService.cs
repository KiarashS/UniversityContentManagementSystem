using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IImageLinkService
    {
        Task AddOrUpdateImageLinkAsync(ImageLinkViewModel imageLink);
        Task<ImageLink> FindImageLinkByIdAsync(long imageLinkId);
        Task<IList<ImageLinkViewModel>> GetPagedImageLinksAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> ImageLinksCountAsync();
        Task<long> ImageLinksPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null);
        Task DeleteImageLinkAsync(long id);
        Task<IList<ViewModels.ImageLinkViewModel>> GetImageLinksAsync(string portalKey, Language language, int maxSize = 4);
        Task<string> GetImageLinkImagenameAsync(long id);
    }
}

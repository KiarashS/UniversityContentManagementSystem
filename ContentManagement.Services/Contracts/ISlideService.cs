using ContentManagement.Entities;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface ISlideService
    {
        Task AddOrUpdateSlideAsync(SlideViewModel slide);
        Task<Slide> FindSlideByIdAsync(long slideId);
        Task<IList<SlideViewModel>> GetPagedSlidesAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> SlideCountAsync();
        Task<long> SlidePagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null);
        Task DeleteSlideAsync(long id);
        Task<string> GetSlideFilenameAsync(long id);
        Task<IList<SliderViewModel>> GetPortalSlidesAsync(string portalKey, Language language, int size);
    }
}

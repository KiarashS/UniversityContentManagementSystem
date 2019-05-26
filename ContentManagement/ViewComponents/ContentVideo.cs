using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class ContentVideo : ViewComponent
    {
        private readonly IContentVideoService _videoService;

        public ContentVideo(IContentVideoService videoService)
        {
            _videoService = videoService;
            _videoService.CheckArgumentIsNull(nameof(videoService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contentId = long.Parse(ViewContext.RouteData.Values["id"].ToString());
            var vm = await _videoService.GetContentVideosAsync(contentId);

            return View(vm);
        }
    }
}

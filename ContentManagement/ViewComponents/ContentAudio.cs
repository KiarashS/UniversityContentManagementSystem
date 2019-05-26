using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;

namespace ContentManagement.ViewComponents
{
    public class ContentAudio : ViewComponent
    {
        private readonly IContentAudioService _audioService;

        public ContentAudio(IContentAudioService audioService)
        {
            _audioService = audioService;
            _audioService.CheckArgumentIsNull(nameof(audioService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contentId = long.Parse(ViewContext.RouteData.Values["id"].ToString());
            var vm = await _audioService.GetContentAudiosAsync(contentId);

            return View(vm);
        }
    }
}

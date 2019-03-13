using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Controllers
{
    public partial class PController : Controller
    {

        public PController()
        {
        }

        public virtual IActionResult Index(string slug)
        {
            return RedirectToActionPermanent("index", "page", new { slug });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Controllers
{
    public partial class CController : Controller
    {

        public CController()
        {
        }

        public virtual IActionResult Index(long id)
        {
            return RedirectToActionPermanent("details", "content", new { id });
        }
    }
}

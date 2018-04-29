using ContentManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class HomeController : Controller
    {
        public virtual IActionResult Index()
        {
            ViewBag.Title = "مدیریت پرتال";
            return View();
        }
    }
}

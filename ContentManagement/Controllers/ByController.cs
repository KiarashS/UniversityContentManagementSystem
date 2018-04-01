using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Controllers
{
    public partial class ByController : Controller
    {
        public virtual IActionResult Index()
        {
            const string result = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta content=\"width=device-width\" name=\"viewport\"><title>Designed and Developed by Kiarash Soleimanzadeh</title><style>div{direction:rtl;text-align:center;font-family:tahoma,arial;width:400px;margin:50px auto}</style></head><body><div><div style=\"padding:20px 10px; border: 2px solid #eee;background-color:#fcfcfc;\">Designed and Developed by <a href=\"http://www.kiarash.pro\" target=\"_blank\" title=\"kiarash.s@hotmail.com\">Kiarash Soleimanzadeh</a></div><a href=\"/\">بازگشت</a></div></body></html>";
            return Content(result, "text/html", System.Text.Encoding.UTF8);
        }
    }
}

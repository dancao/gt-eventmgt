using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}

using Microsoft.AspNetCore.Mvc;

namespace SimpleGoogleWebServices.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
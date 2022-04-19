
using Microsoft.AspNetCore.Mvc;


namespace AdminTemplate.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }

     
    }
}
using Microsoft.AspNetCore.Mvc;

namespace SupplyChain.ClientApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

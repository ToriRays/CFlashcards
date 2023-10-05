using Microsoft.AspNetCore.Mvc;

namespace CFlashcards.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index() 
        {
            return View("Demo");
        }
    }
}

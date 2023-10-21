using Microsoft.AspNetCore.Mvc;

namespace CFlashcards.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Demo() 
        {
            return View("Demo");
        }
    }
}

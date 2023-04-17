using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class IndexingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Indexing() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Indexing(string selected, bool tok)
        {
            return View();
        }
    }
}

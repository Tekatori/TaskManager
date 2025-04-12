using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

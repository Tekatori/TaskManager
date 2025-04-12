using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers
{
    public class AttachmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

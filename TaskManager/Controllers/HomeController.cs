using Microsoft.AspNetCore.Mvc;
using TaskManager.DAL;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.ProjectCount = _context.Projects.Count();
            ViewBag.TaskCount = _context.Tasks.Count();
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.CommentCount = _context.Comments.Count();
            ViewBag.AttachmentCount = _context.Attachments.Count();
            return View();
        }
    }
}

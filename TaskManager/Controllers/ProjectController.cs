using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.BLL;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }
        public IActionResult Index()
        {
            var projects = _projectService.GetAllProject();
            return View(projects);
        }
        [HttpPost]
        public JsonResult Create(Project project)
        {
            if (string.IsNullOrEmpty(project.Name))
            {
                return Json(new { success = false, error = "Tên dự án không được để trống." });
            }
            var currentUser = CookieHelper.GetLoggedUser(User);
            project.CreatedAt = DateTime.Now;
            project.OwnerId = currentUser != null ? currentUser.Id : null;
            var res = _projectService.CreateProject(project);
            if(res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Tạo dự án thất bại" });
        }
        [HttpPost]
        public JsonResult Edit(Project project)
        {
            if (string.IsNullOrEmpty(project.Name))
            {
                return Json(new { success = false, error = "Tên dự án không được để trống." });
            }
            if (project.Id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy dự án" });
            }
            var res = _projectService.UpdateProject(project);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Không có thay đổi" });
        }
        [HttpGet]
        public IActionResult GetProjectById(int id)
        {
            var project = _projectService.GetProject(id);
            if (project == null)
                return Json(new { success = false, error = "Không tìm thấy dự án." });

            return Json(new { success = true, data = project });
        }
        [HttpPost]
        public JsonResult DeleteProject(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy dự án" });
            }
            Project pj = new Project();
            pj.Id = id;
            var res = _projectService.DeleteProject(pj);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Dự án đã tồn tại công việc không thể xoá" });
        }
    }
}

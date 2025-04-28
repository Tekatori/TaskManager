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
        private readonly UserService _userService ;
        public ProjectController(ProjectService projectService, UserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            var projects = _projectService.GetAllProjectByUser(currentUser.Id);

            if(currentUser.Role ==(int)CommonEnums.Role.Admin || currentUser.Role == (int)CommonEnums.Role.Leader)
            {
                var lstteamGroup = _userService.GetALLTeamGroup();
                ViewBag.lstteamGroup = lstteamGroup;
            }         
            return View(projects);
        }
        [HttpGet]
        public IActionResult FilterProject(string textsearch = "")
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            var projects = _projectService.GetAllProjectByUser(currentUser.Id);

            if (!string.IsNullOrWhiteSpace(textsearch) && projects is not null)
            {
                var filteredProjects = projects
                .Where(p => p.Name is not null && p.Name.Contains(textsearch, StringComparison.OrdinalIgnoreCase))
                .ToList();
                projects = filteredProjects;
            }
            if (currentUser.Role == (int)CommonEnums.Role.Admin || currentUser.Role == (int)CommonEnums.Role.Leader)
            {
                var lstteamGroup = _userService.GetALLTeamGroup();
                ViewBag.lstteamGroup = lstteamGroup;
            }
            return PartialView("_ProjectListPartial", projects);
        }


        [HttpPost]
        public JsonResult GetAssignedName(int? pIdProject)
        {
            if (!pIdProject.HasValue)
            {
                return Json(new { success = false, error = "có lỗi xảy ra" });
            }
            string AssignedName = "";
            var project = _projectService.GetProject(pIdProject.Value);
            if(project != null)
            {
                if (project.TeamGroupId.HasValue)
                {
                    var teamgroup = _userService.GetTeamGroup(project.TeamGroupId.Value);
                    if (teamgroup != null)
                    {
                        AssignedName =teamgroup.Name +" (Nhóm)";
                    }
                }
                else 
                {
                    if (project.OwnerId.HasValue)
                    {
                        var user = _userService.GetUser(project.OwnerId.Value);
                        if(user != null)
                        {
                            AssignedName = user.Username + " (cá nhân)"; ;
                        }    
                    }
                }
            }
            else
            {
                return Json(new { success = false, error = "có lỗi xảy ra" });
            }
            return Json(new { success = true, data = AssignedName });
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
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser != null)
            {
                if (currentUser.Id == project.OwnerId || currentUser.Role == (int)CommonEnums.Role.Admin)
                {
                    var res = _projectService.UpdateProject(project);
                    if (res > 0)
                        return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Bạn không có quyền sửa dự án" });
                }
            }
            else
            {
                return Json(new { success = false, error = "Người dùng không hợp lệ" });
            }    
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
            var currentUser = CookieHelper.GetLoggedUser(User);
            Project pj = new Project();
            pj.Id = id;

            var project = _projectService.GetProject(id);
            if(currentUser != null)
            {
                if(project.TeamGroupId != null)
                {
                    if (project.OwnerId == currentUser.Id)
                    {
                        var res = _projectService.DeleteProject(pj);
                        if (res > 0)
                            return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, error = "Bạn không có quyền xoá dự án" });
                    }
                }
                else
                {
                    var res = _projectService.DeleteProject(pj);
                    if (res > 0)
                        return Json(new { success = true });
                }    
            }     
            return Json(new { success = false, error = "Dự án đã tồn tại công việc không thể xoá" });
        }
    }
}

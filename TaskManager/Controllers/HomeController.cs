using Microsoft.AspNetCore.Mvc;
using TaskManager.BLL;
using TaskManager.DAL;
using TaskManager.DAL.ViewModel;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;

        public HomeController(TaskService taskService, ProjectService projectService, UserService userService)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;

        }

        public IActionResult Index()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            TaskParam paramtask = new TaskParam();
            paramtask.IdUser = currentUser.Id;
            paramtask.RoleUser = currentUser.Role;
            var lsttask = _taskService.GetListTask(paramtask);
            var lsttaskdeadline = lsttask.OrderBy(t => t.CreatedAt).Take(5).ToList();
            var lsttaskrecently = lsttask.OrderByDescending(t => t.CreatedAt).Take(5).ToList();

            var projects = _projectService.GetAllProjectByUser(currentUser.Id);

            var users = _userService.GetAllUser();

            DashboardViewModel model = new DashboardViewModel();
            model.CountTask = lsttask.Count();
            model.CountTaskInProcess = lsttask.Where(t=>t.Status == (int)CommonEnums.TaskStatus.InProgress).Count();
            model.CountTaskDone = lsttask.Where(t => t.Status == (int)CommonEnums.TaskStatus.Completed).Count();
            model.CountProject = projects.Count();
            model.CountUser = users.Count();
            model.ListDeadlines = lsttaskdeadline;
            model.ListRecently = lsttaskrecently;

            return View(model);
        }

        [HttpGet]
        public IActionResult GetTaskStatusData()
        {
            int totalcompleted = 0;
            int totalinProgress = 0;
            int totalNewTask = 0;
            var currentUser = CookieHelper.GetLoggedUser(User);
            if(currentUser == null)
                return Json(new { success = false, error = "Người dùng không hợp lệ" });
            TaskParam paramtask = new TaskParam();
            paramtask.IdUser = currentUser.Id;
            paramtask.RoleUser = currentUser.Role;
            var lsttask = _taskService.GetListTask(paramtask);
            if(lsttask != null && lsttask.Count > 0)
            {
                totalcompleted = lsttask.Count(t => t.Status == (int)CommonEnums.TaskStatus.Completed);
                totalinProgress = lsttask.Count(t => t.Status == (int)CommonEnums.TaskStatus.InProgress);
                totalNewTask = lsttask.Count(t => t.Status == (int)CommonEnums.TaskStatus.New);
            }

            var data = new
            {
                labels = new[] { CommonEnums.GetDescription(CommonEnums.TaskStatus.Completed), CommonEnums.GetDescription(CommonEnums.TaskStatus.InProgress), CommonEnums.GetDescription(CommonEnums.TaskStatus.New) },
                values = new[] { totalcompleted, totalinProgress, totalNewTask }
            };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetTasksByProjectData()
        {
            var labelsproject = new List<string>();
            var valuesproject = new List<string>();

            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return Json(new { success = false, error = "Người dùng không hợp lệ" });
            var projects = _projectService.GetAllProjectByUser(currentUser.Id);
            foreach(var item in projects)
            {
                labelsproject.Add(item.Name);
                var counttask = _taskService.GetAllTaskInProject(item.Id);
                if (counttask != null && counttask.Count > 0)
                {
                    valuesproject.Add(counttask.Count.ToString());
                }
                else
                {
                    valuesproject.Add("0");
                }
            }    
            var data = new
            {
                labels = labelsproject,
                values = valuesproject
            };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetLatestComments()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return Json(new { success = false, error = "Người dùng không hợp lệ" });

            TaskParam param = new TaskParam();
            param.IdUser = currentUser.Id;
            param.RoleUser = currentUser.Role;

            var comments = _taskService.GetAllCommentsByIdUser(param); 
            var latestComments = comments
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)  
                .Select(c => new
                {
                    c.UserName,
                    c.TaskName,
                    c.TaskId,
                    c.Content,
                    c.CreatedAt
                })
                .ToList();

            return Json(new { comments = latestComments });
        }
    }
}

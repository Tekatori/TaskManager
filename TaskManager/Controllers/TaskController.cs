using Microsoft.AspNetCore.Mvc;
using TaskManager.BLL;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TaskController : Controller
    {

        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;

        public TaskController(TaskService taskService, ProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }
        public IActionResult Index(TaskParam param)
        {
            var lstProjectName = _projectService.GetAllProject();
            ViewBag.ListProject = lstProjectName;

            var lsttask = new List<TaskItem>();

            if(param.IdProject != 0)
            {
                lsttask = _taskService.GetAllTaskInProject(param.IdProject);
            }
            else
            {
                lsttask = _taskService.GetAllTask();
            }
            if(!string.IsNullOrEmpty(param.textsearch))
            {
                lsttask = lsttask.Where(t => t.Title != null && t.Title.ToLower().Contains(param.textsearch.ToLower())).ToList();
            }    
            return View(lsttask);
        }
        [HttpPost]
        public JsonResult Create(TaskItem taskItem)
        {
            if (string.IsNullOrEmpty(taskItem.Title))
            {
                return Json(new { success = false, error = "Tên công việc không được để trống." });
            }
            var currentUser = CookieHelper.GetLoggedUser(User);
            taskItem.CreatedAt = DateTime.Now;
            taskItem.AssignedTo = currentUser != null ? currentUser.Id : null;
            var res = _taskService.CreateTask(taskItem);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Tạo dự án thất bại" });
        }
        [HttpPost]
        public JsonResult Edit(TaskItem taskItem)
        {
            if (string.IsNullOrEmpty(taskItem.Notes))
            {
                return Json(new { success = false, error = "ghi chú không được để trống" });
            }
            if (taskItem.Id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy công việc" });
            }

            var taskupdate = _taskService.GetTask(taskItem.Id);
            taskupdate.Status = taskItem.Status;
            taskupdate.Notes = taskItem.Notes;
            taskupdate.UpdatedAt = DateTime.Now;

            var res = _taskService.UpdateTask(taskupdate);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Không có thay đổi" });
        }
        [HttpGet]
        public IActionResult GetTaskById(int id)
        {
            var task = _taskService.GetTask(id);
            if (task == null)
                return Json(new { success = false, error = "Không tìm thấy công việc." });

            return Json(new { success = true, data = task });
        }
        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy công việc" });
            }

            var task = _taskService.GetTask(id);
            if (task == null)
                return Json(new { success = false, error = "Không tìm thấy công việc." });

            if (task.Status == (int)CommonEnums.TaskStatus.InProgress)
            {
                return Json(new { success = false, error = "Không thể xóa công việc đang xử lý." });
            }
            if (task.Status == (int)CommonEnums.TaskStatus.Completed)
            {
                return Json(new { success = false, error = "Không thể xóa công việc đã hoàn thành." });
            }



            var res = _taskService.DeleteTask(task);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Xoá thất bại" });
        }
    }
}

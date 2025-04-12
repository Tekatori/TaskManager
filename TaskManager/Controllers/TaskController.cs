using Microsoft.AspNetCore.Mvc;
using TaskManager.BLL;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TaskController : Controller
    {

        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }
        public IActionResult Index(int id)
        {
            var lsttask = _taskService.GetAllTaskInProject(id);
            string ProjectName = _taskService.GetProjectName(id);
            ViewBag.ProjectName = ProjectName;
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
            if (string.IsNullOrEmpty(taskItem.Title))
            {
                return Json(new { success = false, error = "Tên dự án không được để trống." });
            }
            if (taskItem.Id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy công việc" });
            }
            taskItem.UpdatedAt = DateTime.Now;
            var res = _taskService.UpdateTask(taskItem);
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
            TaskItem task = new TaskItem();
            task.Id = id;
            var res = _taskService.DeleteTask(task);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Xoá thất bại" });
        }
    }
}

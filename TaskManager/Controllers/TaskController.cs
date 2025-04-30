using Microsoft.AspNetCore.Mvc;
using TaskManager.BLL;
using TaskManager.Models;
using TaskManager.DAL.ViewModel;
namespace TaskManager.Controllers
{
    public class TaskController : Controller
    {
        #region Khởi tạo
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;
      
        public TaskController(TaskService taskService, ProjectService projectService, UserService userService)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;
        }
        #endregion

        #region Công việc
        public IActionResult Index()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            var lstProjectName = _projectService.GetAllProjectByUser(currentUser.Id);
            ViewBag.ListProject = lstProjectName;

            TaskParam param = new TaskParam();
            param.IdUser = currentUser.Id;
            param.RoleUser = currentUser.Role;

            var lsttask = _taskService.GetAllTaskNotDone(param);
            return View(lsttask);
        }
        [HttpGet]
        public IActionResult FilterTask(TaskParam param)
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            param.IdUser = currentUser.Id;
            param.RoleUser = currentUser.Role;

            var lstProjectName = _projectService.GetAllProjectByUser(currentUser.Id);
            ViewBag.ListProject = lstProjectName;

            var lsttask = _taskService.GetListTask(param);
            return PartialView("_TaskListPartial", lsttask);
        }
        [HttpGet]
        public IActionResult GetTeamGroupByIdProject(int? pIdProject)
        {
            if(pIdProject.HasValue)
            {
                var currentUser = CookieHelper.GetLoggedUser(User);
                var project = _projectService.GetProject(pIdProject.Value);
                if (project != null && project.TeamGroupId.HasValue)
                {
                    var lstUser = _userService.GetUserByTeamGroupId(project.TeamGroupId.Value);
                    if (lstUser != null && currentUser != null)
                    {
                        lstUser = lstUser.Where(t=>t.Id != currentUser.Id).ToList();
                    }
                    return Json(new { success = true, data = lstUser });
                }
                else
                {
                    return Json(new { success = false, error = "Có lỗi xảy ra" });
                }
            }
            return Json(new { success = false, error = "Có lỗi xảy ra" });
        }
        [HttpPost]
        public JsonResult Create(TaskItem taskItem,IFormFile attachment)
        {
            if (string.IsNullOrEmpty(taskItem.Title))
            {
                return Json(new { success = false, error = "Tên công việc không được để trống." });
            }
            var currentUser = CookieHelper.GetLoggedUser(User);
            taskItem.CreatedAt = DateTime.Now;

            if (!taskItem.AssignedTo.HasValue || taskItem.AssignedTo == 0)
            {
                taskItem.AssignedTo = currentUser != null ? currentUser.Id : null;
            }
            var res = _taskService.CreateTask(taskItem);
            if (res > 0)
            {
                string filename = FileHelper.SaveFile(attachment);

                if(!string.IsNullOrEmpty(filename))
                {
                    Attachment attach = new Attachment();
                    attach.TaskId = taskItem.Id;
                    attach.FileName = filename;
                    attach.FileUrl = "/uploads/" + filename;
                    attach.UploadedAt = DateTime.Now;
                    var resAttach = _taskService.SaveAttachmentTask(attach);
                }
                return Json(new { success = true });
            }    
                
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
        [HttpPost]
        public JsonResult EditWithId(TaskItem taskItem)
        {
            if (string.IsNullOrEmpty(taskItem.Notes))
            {
                return Json(new { success = false, error = "Ghi chú không được để trống" });
            }
            if (taskItem.Id <= 0)
            {
                return Json(new { success = false, error = "Không tìm thấy công việc" });
            }

            var taskUpdate = _taskService.GetTask(taskItem.Id);
            taskUpdate.Status = taskItem.Status;
            taskUpdate.Notes = taskItem.Notes;
            taskUpdate.UpdatedAt = DateTime.Now;

            var res = _taskService.UpdateTask(taskUpdate);
            if (res > 0)
            {
                return Json(new { success = true, data = new { Id = taskItem.Id, Status = taskItem.Status } });
            }
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
            var currentUser = CookieHelper.GetLoggedUser(User);


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
            if(currentUser != null && task.ProjectId.HasValue)
            {
                var project = _projectService.GetProject(task.ProjectId.Value);
                if(project != null && project.OwnerId != currentUser.Id && currentUser.Id != (int)CommonEnums.Role.Admin)
                {
                    return Json(new { success = false, error = "Bạn không có quyền xóa công việc này." });
                }       
            }

            var res = _taskService.DeleteTask(task);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Xoá thất bại" });
        }
        [HttpPost]
        public JsonResult GetTaskAssigneeName(int? AssignedTo)
        {
            if (!AssignedTo.HasValue)
            {
                return Json(new { success = false, error = "có lỗi xảy ra" });
            }
            string AssignedName = "";
            var user = _userService.GetUser(AssignedTo.Value);
            if (user != null)
            {
                AssignedName = $"{user.Username} ({user.Email})";
            }
            else
            {
                return Json(new { success = false, error = "có lỗi xảy ra" });
            }
            return Json(new { success = true, data = AssignedName });
        }
        [HttpGet]
        public JsonResult GetCommentsByTask(int taskId)
        {
            var comments = _taskService.GetCommentsByIdTask(new CommentParam { TaskId = taskId });
            return Json(new { success = true, data = comments });
        }

        [HttpPost]
        public JsonResult AddComment(CommentViewModel model)
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if(currentUser == null)
            {
                return Json(new { success = false, error = "Không thể thêm bình luận" });
            }
            if (currentUser.Id.HasValue)
            {
                model.UserId = currentUser.Id.Value;
            }
            var result = _taskService.SaveCommentTask(model);
            if (result > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Không thể thêm bình luận" });
        }
        public IActionResult GetAttachments(int taskId)
        {
            var attachment = _taskService.GetAttachmentsByTaskId(taskId); 

            if (attachment != null)
            {
                var attachmentData = new
                {
                    attachment.Id,
                    attachment.FileName,
                    attachment.FileUrl
                };

                return Json(new { success = true, attachment = attachmentData });
            }

            return Json(new { success = false, error = "Không có đính kèm nào." });
        }
        [HttpPost]
        public JsonResult EditAttachment(int taskId, int IdAttachment, IFormFile newAttachment)
        {
            if (newAttachment != null)
            {
                var attach = _taskService.GetAttachmentsByTaskId(taskId);
                if (attach != null)
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attach.FileUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    string filename = FileHelper.SaveFile(newAttachment);
                    if (string.IsNullOrEmpty(filename))
                    {
                        return Json(new { success = false, error = "Không thể lưu tệp đính kèm mới." });
                    }

                    attach.FileName = filename;
                    attach.FileUrl = "/uploads/" + filename;
                    attach.UploadedAt = DateTime.Now;

                    var res = _taskService.SaveAttachmentTask(attach);
                    if (res > 0)
                    {
                        return Json(new { success = true });
                    }
                }
            }
            return Json(new { success = false, error = "Vui lòng chọn đính kèm." });
        }
        [HttpPost]
        public JsonResult AddAttachment(int taskId, IFormFile newAttachmentFile)
        {
            if (newAttachmentFile != null)
            {
                string filename = FileHelper.SaveFile(newAttachmentFile);
                if (string.IsNullOrEmpty(filename))
                {
                    return Json(new { success = false, error = "Không thể lưu tệp đính kèm." });
                }

                var attachment = new Attachment
                {
                    TaskId = taskId,
                    FileName = filename,
                    FileUrl = "/uploads/" + filename,
                    UploadedAt = DateTime.Now
                };

                var res = _taskService.SaveAttachmentTask(attachment);
                if (res > 0)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, error = "Có lỗi xảy ra khi thêm đính kèm." });
        }
        [HttpPost]
        public JsonResult DeleteAttachment(int taskId, int attachmentId)
        {
            var attachment = _taskService.GetAttachmentsByTaskId(taskId);
            if (attachment != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FileUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);  
                }

                var result = _taskService.DeleteAttachment(attachment);
                if (result > 0)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, error = "Có lỗi xảy ra khi xoá đính kèm." });
        }

        #endregion

        #region Lịch
        public IActionResult Calendar()
        {

            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            var lstProjectName = _projectService.GetAllProjectByUser(currentUser.Id);
            ViewBag.ListProject = lstProjectName;

            var curentDate = DateTime.Now;

            TaskParam param = new TaskParam();
            param.IdUser = currentUser.Id;
            param.RoleUser = currentUser.Role;

            var lsttask = _taskService.GetAllTaskNotDone(param);


            var events = lsttask
             .Where(t => t.DueDate.HasValue && t.DueDate.Value.Month == curentDate.Month)
             .Select(t => new CalendarEvent
             {
                 IdTask = t.Id,
                 Title = t.Title,
                 StartDate = t.DueDate ?? DateTime.MinValue,
                 ColorClass = t.Status switch
                 {
                     0 => "calendar-event-new",
                     1 => "calendar-event-process",
                     2 => "calendar-event-done",
                     _ => "calendar-event-default"
                 }
             })
             .ToList();

            var viewModel = new CalendarEventViewModel
            {
                StartDate = new DateTime(curentDate.Year, curentDate.Month, 1),
                Events = events
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GetCalendarEventsByMonth(CalendarParam param)
        {

            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");

            var lstProjectName = _projectService.GetAllProjectByUser(currentUser.Id);
            ViewBag.ListProject = lstProjectName;

            var curentDate = DateTime.Now;

            TaskParam tparam = new TaskParam();
            tparam.IdUser = currentUser.Id;
            tparam.RoleUser = currentUser.Role;
            tparam.IdProject = param.IdProject;

            var lsttask = _taskService.GetListTask(tparam);

            var events = lsttask
             .Where(t => t.DueDate.HasValue && t.DueDate.Value.Month == param.month && t.DueDate.Value.Year == param.year)
             .Select(t => new CalendarEvent
             {
                 IdTask = t.Id,
                 Title = t.Title,
                 StartDate = t.DueDate ?? DateTime.MinValue,
                 ColorClass = t.Status switch
                 {
                     0 => "calendar-event-new",
                     1 => "calendar-event-process",
                     2 => "calendar-event-done",
                     _ => "calendar-event-default"
                 }
             })
             .ToList();

            var viewModel = new CalendarEventViewModel
            {
                StartDate = new DateTime(param.year, param.month, 1),
                Events = events
            };

            return PartialView("_FilterCalendar", viewModel);
        }

        #endregion

    }
}

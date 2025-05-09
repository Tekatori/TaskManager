﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DAL.ViewModel;
using TaskManager.Models;
namespace TaskManager.DAL
{
    public class TaskRepository
    {
        private readonly AppDbContext _context;

        #region Contructor
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Public
        public List<TaskItem> GetAllTask()
        {
            var us = _context.Tasks.ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<TaskItem>();
        }
        public List<TaskItem> GetAllTaskNotDone(TaskParam param)
        {
            List<int> lstIdProject = GetAllListIdProjectUser(param.IdUser);
                      
            var us = _context.Tasks.Where(t => t.Status != (int)CommonEnums.TaskStatus.Completed && t.ProjectId.HasValue && lstIdProject.Contains(t.ProjectId.Value)).OrderBy(t => t.Status).ToList();

            if (param.RoleUser == (int)CommonEnums.Role.User)
            {
                us = us.Where(t => t.AssignedTo == param.IdUser).ToList();
            }

            if (us != null && us.Count() > 0)
                return us;
            return new List<TaskItem>();
        }
        
        public List<TaskItem> GetAllTaskByIdStatus(int? pIdStatus)
        {
            var us = _context.Tasks.Where(t => t.Status == pIdStatus).ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<TaskItem>();
        }
        public List<TaskItem> GetListTask(TaskParam param)
        {
            List<int> lstIdProject = GetAllListIdProjectUser(param.IdUser);

            var us = _context.Tasks.Where(t => t.ProjectId.HasValue && lstIdProject.Contains(t.ProjectId.Value)).OrderBy(t=>t.Status).ToList();

            if(param.RoleUser == (int)CommonEnums.Role.User)
            {
                us = us.Where(t => t.AssignedTo == param.IdUser).ToList();
            }    

            if (param.IdProject.HasValue)
            {
                us = us.Where(t => t.ProjectId == param.IdProject).ToList();
            }
            if(param.IdStatus.HasValue && param.IdStatus != (int)CommonEnums.TaskStatus.All)
            {
                if (param.IdStatus == (int)CommonEnums.TaskStatus.NotCompleted)
                {
                    us = us.Where(t => t.Status != (int)CommonEnums.TaskStatus.Completed).ToList();
                }
                else 
                {
                    us = us.Where(t => t.Status == param.IdStatus).ToList();
                }        
            }
            if(!string.IsNullOrEmpty(param.textsearch))
            {
                us = us.Where(t => t.Title != null && t.Title.ToLower().Contains(param.textsearch.ToLower())).ToList();
            }
            if (us != null && us.Count() > 0)
                return us;
            return new List<TaskItem>();
        }
        public TaskItem GetTask(int? pId)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == pId);
            if (task != null) return task;
            return new TaskItem();
        }
        public List<TaskItem> GetAllTaskInProject(int? pIdProject)
        {
            var lsttask = _context.Tasks.Where(x => x.ProjectId == pIdProject).ToList();
            if (lsttask != null && lsttask.Count() > 0) return lsttask;
            return new List<TaskItem>();
        }

        public string GetProjectName(int? pIdProject) 
        {
            var pj = _context.Projects.FirstOrDefault(t=>t.Id == pIdProject);
            if(pj != null) return pj.Name;
            return string.Empty;
        }

        public int CreateTask(TaskItem pTask)
        {
            pTask.Id = 0;
            return SaveData(pTask);
        }
        public int UpdateTask(TaskItem pTask)
        {
            return SaveData(pTask);
        }
        public int DeleteTask(TaskItem pTask)
        {
            return SaveData(pTask, true);
        }
        public List<CommentViewModel> GetCommentsByIdTask(CommentParam param)
        {
            if (param == null || !param.TaskId.HasValue)
                return new List<CommentViewModel>();

            var lstComment = _context.Comments.Where(t => t.TaskId == param.TaskId).ToList();
            var lstcommentV = ExtensionClass.ConvertList<Comment, CommentViewModel>(lstComment);

            if (lstcommentV?.Count > 0)
            {
                foreach (var item in lstcommentV)
                {
                    var user = _context.Users.FirstOrDefault(t => t.Id == item.UserId);
                    if (user != null)
                    {
                        item.UserName = $"{user.Username} ({user.Email})";
                    }
                }
            }

            return lstcommentV ?? new List<CommentViewModel>();
        }
        public List<CommentViewModel> GetAllCommentsByIdUser(TaskParam param)
        {
            var lstidtask = GetListTask(param).Select(t=>t.Id).ToList();
            var lstComment = _context.Comments.OrderByDescending(t=>t.CreatedAt).Where(t => lstidtask.Contains(t.TaskId)).Take(5).ToList();

            var lstcommentV = ExtensionClass.ConvertList<Comment, CommentViewModel>(lstComment);

            if (lstcommentV?.Count > 0)
            {
                foreach (var item in lstcommentV)
                {
                    var user = _context.Users.FirstOrDefault(t => t.Id == item.UserId);
                    if (user != null)
                    {
                        item.UserName = $"{user.Username} ({user.Email})";
                    }
                    var task = _context.Tasks.FirstOrDefault(t => t.Id == item.TaskId);
                    if (task != null)
                    {
                        item.TaskName = task.Title;
                    }
                }
            }

            return lstcommentV ?? new List<CommentViewModel>();
        }


        public int SaveCommentTask(CommentViewModel model)
        {
            int res = 0;
            if(model.Id == 0 && model.TaskId != 0 && model.UserId != 0 && !string.IsNullOrEmpty(model.Content))
            {
                Comment comment = new Comment();
                comment.TaskId = model.TaskId;
                comment.UserId = model.UserId;
                comment.Content = model.Content;
                comment.CreatedAt = DateTime.Now;
                _context.Comments.Add(comment);
            }
            else
            {
                var findcomment = _context.Comments.FirstOrDefault(t => t.Id == model.Id);
                if(findcomment != null)
                {
                    findcomment.Content = model.Content;
                }
            }
            res = _context.SaveChanges();
            return res;
        }
        public int SaveAttachmentTask(Attachment model)
        {
            int res = 0;
            if (model.Id == 0 && model.TaskId != 0 && !string.IsNullOrEmpty(model.FileName))
            {
                _context.Attachments.Add(model);
            }
            else
            {
                var findcomment = _context.Attachments.FirstOrDefault(t => t.Id == model.Id);
                if (findcomment != null)
                {
                    findcomment.FileName = model.FileName;
                }
            }
            res = _context.SaveChanges();
            return res;
        }
        public Attachment GetAttachmentsByTaskId(int? pIdTask)
        {
            var attachment = _context.Attachments.FirstOrDefault(t => t.TaskId == pIdTask);
            if (attachment != null)
                return attachment;
            return null;
        }
        public int DeleteAttachment(Attachment attachment)
        {
            if(attachment != null)
            {
                _context.Attachments.Remove(attachment);
                return _context.SaveChanges();
            }
            return 0;
        }
        #endregion

        #region Private
        private int SaveData(TaskItem pTask, bool pIsDelete = false)
        {
            int res = 0;
            var taskItem = _context.Tasks.FirstOrDefault(t => t.Id == pTask.Id);
            if (taskItem != null && pTask.Id != 0 && !pIsDelete)
            {
                taskItem.Title = pTask.Title;
                taskItem.Description = pTask.Description;
                taskItem.Status = pTask.Status;
                taskItem.Priority = pTask.Priority;
                taskItem.DueDate = pTask.DueDate;
                taskItem.UpdatedAt = pTask.UpdatedAt;
                taskItem.ProjectId = pTask.ProjectId;
                taskItem.AssignedTo = pTask.AssignedTo;
                taskItem.Notes = pTask.Notes;
            }
            else if (pIsDelete == true && taskItem != null)
            {
                var findcomment = _context.Comments.Where(t => t.TaskId == taskItem.Id).ToList();
                if (findcomment != null && findcomment.Count() > 0)
                {
                    _context.Comments.RemoveRange(findcomment);
                }

                var findAttachments = _context.Attachments.Where(t => t.TaskId == taskItem.Id).ToList();
                if (findAttachments != null && findAttachments.Count() > 0)
                {
                    _context.Attachments.RemoveRange(findAttachments);
                }

                _context.Tasks.Remove(taskItem);
            }
            else
            {
                _context.Tasks.Add(pTask);
            }
            res = _context.SaveChanges();
            return res;
        }
        private List<int> GetAllListIdProjectUser(int? pIDUser)
        {
            if (pIDUser == null)
                return new List<int>();

            var user = _context.Users.FirstOrDefault(t => t.Id == pIDUser.Value);
            if (user == null)
                return new List<int>();

            var userId = user.Id;

            var ownedProjects = _context.Projects
                .Where(p => p.OwnerId == userId)
                .ToList();

            var teamGroups = _context.TeamGroup
                .Where(tg => !string.IsNullOrWhiteSpace(tg.ListIdUser))
                .AsEnumerable()
                .Where(tg => tg.ListIdUser.ToIntList().Contains(userId))
                .Select(tg => tg.Id)
                .ToList();

            var teamProjects = _context.Projects
                .Where(p => p.TeamGroupId.HasValue && teamGroups.Contains(p.TeamGroupId.Value))
                .ToList();

            var allProjects = ownedProjects
                .Union(teamProjects)
                .ToList();

            return allProjects.Select(t=>t.Id).ToList();
        }
        #endregion
    }

}

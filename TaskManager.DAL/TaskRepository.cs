using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                _context.Tasks.Remove(taskItem);
            }
            else
            {
                _context.Tasks.Add(pTask);
            }
            res = _context.SaveChanges();
            return res;
        }
        #endregion
    }

}

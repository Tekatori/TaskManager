using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DAL;
using TaskManager.DAL.ViewModel;
using TaskManager.Models;

namespace TaskManager.BLL
{
    public class TaskService
    {
        private readonly TaskRepository _repo;

        public TaskService(TaskRepository taskRepository)
        {
            _repo = taskRepository;
        }

        #region GET Item
        public List<TaskItem> GetAllTask()
        {
            return _repo.GetAllTask();
        }
        public List<TaskItem> GetAllTaskNotDone(int? pIdUser)
        {
            return _repo.GetAllTaskNotDone(pIdUser);
        }
        public List<TaskItem> GetAllTaskByIdStatus(int? pIdStatus)
        {
            return _repo.GetAllTaskByIdStatus(pIdStatus);
        }
        public TaskItem GetTask(int? pId)
        {
            return _repo.GetTask(pId);
        }
        public List<TaskItem> GetAllTaskInProject(int? pIdProject)
        {
            return _repo.GetAllTaskInProject(pIdProject);
        }
        public string GetProjectName(int? pIdProject)
        {
            return _repo.GetProjectName(pIdProject);
        }
        public List<TaskItem> GetListTask(TaskParam param)
        {
            return _repo.GetListTask(param);
        }
        #endregion

        #region CRUD
        public int CreateTask(TaskItem pTask)
        {
            return _repo.CreateTask(pTask);
        }
        public int UpdateTask(TaskItem pTask)
        {
            return _repo.UpdateTask(pTask);
        }
        public int DeleteTask(TaskItem pTask)
        {
            return _repo.DeleteTask(pTask);
        }

        #endregion
    }
}

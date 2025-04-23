using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DAL;
using TaskManager.Models;

namespace TaskManager.BLL
{
    public class ProjectService
    {
        private readonly ProjectRepository _repo;

        public ProjectService(ProjectRepository projectRepository)
        {
            _repo = projectRepository;
        }

        #region GET Item
        public List<Project> GetAllProject()
        {
            return _repo.GetAllProject();
        }
        public List<Project> GetAllProjectByUser(int? pIdUser)
        {
            return _repo.GetAllProjectByUser(pIdUser);
        }
        public List<Project> GetAllProjectByTeamGroup(int? pIdTeamGroup)
        {
            return _repo.GetAllProjectByTeamGroup(pIdTeamGroup);
        }
        public Project GetProject(int pId)
        {
            return _repo.GetProject(pId);
        }
        #endregion

        #region CRUD
        public int CreateProject(Project pProject)
        {
            return _repo.CreateProject(pProject);
        }
        public int UpdateProject(Project pProject)
        {
            return _repo.UpdateProject(pProject);
        }
        public int DeleteProject(Project pProject)
        {
            return _repo.DeleteProject(pProject);
        }

        #endregion
    }
}

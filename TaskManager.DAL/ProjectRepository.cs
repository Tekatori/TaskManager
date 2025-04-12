using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.DAL
{
    public class ProjectRepository
    {
        private readonly AppDbContext _context;

        #region Contructor
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Public
        public List<Project> GetAllProject()
        {
            var pj = _context.Projects.ToList();
            if (pj != null && pj.Count() > 0)
                return pj;
            return new List<Project>();
        }
        public Project GetProject(int pId)
        {
            var pj = _context.Projects.FirstOrDefault(x => x.Id == pId);
            if (pj != null) return pj;
            return new Project();
        }

        public int CreateProject(Project pProject)
        {
            pProject.Id = 0;
            return SaveData(pProject);
        }
        public int UpdateProject(Project pProject)
        {
            return SaveData(pProject);
        }
        public int DeleteProject(Project pProject)
        {
            return SaveData(pProject, true);
        }

        #endregion

        #region Private
        private int SaveData(Project pProject, bool pIsDelete = false)
        {
            int res = 0;
            var Pj = _context.Projects.FirstOrDefault(t => t.Id == pProject.Id);
            if (Pj != null && pProject.Id != 0 && !pIsDelete)
            {
                Pj.Name = pProject.Name;
                Pj.Description = pProject.Description;
            }
            else if (pIsDelete == true && Pj != null)
            {
                //tạm chưa xoá task,comment,attacment

                var taskPjExits = _context.Tasks.FirstOrDefault(t=>t.ProjectId == pProject.Id);
                if (taskPjExits != null)
                {
                    return res;
                }
                else
                {
                    _context.Projects.Remove(Pj);
                }
            }
            else
            {
                _context.Projects.Add(pProject);
            }
            res = _context.SaveChanges();
            return res;
        }
        #endregion
    }
}

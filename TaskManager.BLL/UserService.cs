using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.DAL;
using TaskManager.Models;

namespace TaskManager.BLL
{
    public class UserService
    {
        private readonly UserRespository _repo;

        public UserService(UserRespository userRespository)
        {
            _repo = userRespository;
        }

        #region GET Item
        public List<User> GetAllUser()
        {
            return _repo.GetAllUser();
        }
        public User GetUser(int pId)
        {
           return _repo.GetUser(pId);
        }
        public User GetUser(string pUserName)
        {
           return _repo.GetUser(pUserName);
        }
        public User GetUserByEmail(string pEmail)
        {
            return _repo.GetUserByEmail(pEmail);
        }
        public bool isExitEmailUser(string pEmail)
        {
           return _repo.isExitEmailUser(pEmail);
        }
        public bool isExitUserName(string pUserName)
        {
            return _repo.isExitUserName(pUserName);
        }
        public List<TeamGroup> GetALLTeamGroup()
        {
            return _repo.GetALLTeamGroup();
        }
        public TeamGroup GetTeamGroup(int pId)
        {
            return _repo.GetTeamGroup(pId);
        }
        public bool isExitTeamGroup(string pName)
        {
            return _repo.isExitTeamGroup(pName);
        }
        public List<User> GetUserByTeamGroupId(int pTeamGroupId)
        {
            return _repo.GetUserByTeamGroupId(pTeamGroupId);
        }
        public List<User> GetUsersExcludingTeamGroupId(int pTeamGroupId)
        {
            return _repo.GetUsersExcludingTeamGroupId(pTeamGroupId);
        }
        #endregion

        #region CRUD
        public int CreateUser(User pUser)
        {
            return _repo.CreateUser(pUser);
        }
        public int UpdateUser(User pUser)
        {
            return _repo.UpdateUser(pUser);
        }
        public int DeleteUser(User pUser)
        {
            return _repo.DeleteUser(pUser);
        }
        public int CreateTeamGroup(TeamGroup pTeamGroup)
        {
            return _repo.CreateTeamGroup(pTeamGroup);
        }
        public int UpdateTeamGroup(TeamGroup pTeamGroup)
        {
            return _repo.UpdateTeamGroup(pTeamGroup);
        }
        public int DeleteTeamGroup(TeamGroup pTeamGroup)
        {
            return _repo.DeleteTeamGroup(pTeamGroup);
        }
        public int AddUsertoTeamGroup(List<int>? pUserIds, int pTeamGroupId)
        {
            return _repo.AddUsertoTeamGroup(pUserIds, pTeamGroupId);
        }
        public int UpdateRoleUser(int? pIdUser, int? pRole)
        {
            return _repo.UpdateRoleUser(pIdUser, pRole);
        }
        public int ChangePassWord(int? pIdUser, string pPasswordNewHash)
        {
            return _repo.ChangePassWord(pIdUser, pPasswordNewHash);
        }
        #endregion
    }
}

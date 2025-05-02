using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.DAL;
using TaskManager.DAL.ViewModel;
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
        public List<Users> GetAllUser()
        {
            return _repo.GetAllUser();
        }
        public Users GetUser(int pId)
        {
           return _repo.GetUser(pId);
        }
        public Users GetUser(string pUserName)
        {
           return _repo.GetUser(pUserName);
        }
        public Users GetUserByEmail(string pEmail)
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

        public List<TeamGroup> GetAllTeamGroupByAccount(int? pIdUser)
        {
            return _repo.GetAllTeamGroupByAccount(pIdUser);
        }
        public TeamGroup GetTeamGroup(int pId)
        {
            return _repo.GetTeamGroup(pId);
        }
        public bool isExitTeamGroup(string pName)
        {
            return _repo.isExitTeamGroup(pName);
        }
        public List<Users> GetUserByTeamGroupId(int pTeamGroupId)
        {
            return _repo.GetUserByTeamGroupId(pTeamGroupId);
        }
        public List<Users> GetUsersExcludingTeamGroupId(int pTeamGroupId)
        {
            return _repo.GetUsersExcludingTeamGroupId(pTeamGroupId);
        }
        public Users ForgotPasswordAccount(UserViewModel account)
        {
            return _repo.ForgotPasswordAccount(account);
        }
        public Users ExpTokenResetPasswordAccount(UserViewModel account)
        {
            return _repo.ExpTokenResetPasswordAccount(account);
        }
        public int ResetPasswordAccount(ResetPasswordViewModel model)
        {
            return _repo.ResetPasswordAccount(model);
        }
        #endregion

        #region CRUD
        public int CreateUser(Users pUser)
        {
            return _repo.CreateUser(pUser);
        }
        public int UpdateUser(Users pUser)
        {
            return _repo.UpdateUser(pUser);
        }
        public int DeleteUser(Users pUser)
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

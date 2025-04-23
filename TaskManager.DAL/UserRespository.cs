using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;
namespace TaskManager.DAL
{
    public class UserRespository
    {
        private readonly AppDbContext _context;

        #region Contructor
        public UserRespository(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Public
        public List<User> GetAllUser()
        {
            var us = _context.Users.ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<User>();
        }
        public User GetUser(int pId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == pId);
            if (user != null) return user;
            return new User();
        }
        public User GetUser(string pUserName)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == pUserName);
            if (user != null) return user;
            return new User();
        }
        public User GetUserByEmail(string pEmail)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == pEmail);
            if (user != null) return user;
            return new User();
        }

        public bool isExitEmailUser(string pEmail)
        {
            bool isExit = false;
            var user = _context.Users.FirstOrDefault(x => x.Email == pEmail);
            if (user != null)
                isExit = true;
            return isExit;
        }
        public bool isExitUserName(string pUserName)
        {
            bool isExit = false;
            var user = _context.Users.FirstOrDefault(x => x.Username == pUserName);
            if (user != null)
                isExit = true;
            return isExit;
        }
        public int CreateUser(User pUser)
        {
            pUser.Id = 0;
            return SaveData(pUser);
        }
        public int UpdateUser(User pUser)
        {
            return SaveData(pUser);
        }
        public int DeleteUser(User pUser)
        {
            return SaveData(pUser, true);
        }
        public List<User> GetUserByTeamGroupId(int pTeamGroupId)
        {
            var us = _context.Users.Where(x => x.TeamGroupId == pTeamGroupId).ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<User>();
        }
        public List<User> GetUsersExcludingTeamGroupId(int pTeamGroupId)
        {
            var us = _context.Users.Where(x => x.TeamGroupId != pTeamGroupId).ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<User>();
        }
        public List<TeamGroup> GetALLTeamGroup()
        {
            var tg = _context.TeamGroup.ToList();
            if (tg != null && tg.Count() > 0)
                return tg;
            return new List<TeamGroup>();
        }
        public TeamGroup GetTeamGroup(int pId)
        {
            var tg = _context.TeamGroup.FirstOrDefault(x => x.Id == pId);
            if (tg != null)
                return tg;
            return new TeamGroup();
        }
        public bool isExitTeamGroup(string pName)
        {
            bool isExit = false;
            var tg = _context.TeamGroup.FirstOrDefault(x => x.Name == pName);
            if (tg != null)
                isExit = true;
            return isExit;
        }
        public int CreateTeamGroup(TeamGroup pTeamGroup)
        {
            pTeamGroup.Id = 0;
            return SaveData(pTeamGroup);
        }
        public int UpdateTeamGroup(TeamGroup pTeamGroup)
        {
            return SaveData(pTeamGroup);
        }
        public int DeleteTeamGroup(TeamGroup pTeamGroup)
        {
            return SaveData(pTeamGroup, true);
        }
        public int AddUsertoTeamGroup(int pUserId, int pTeamGroupId)
        {
            return SaveUsertoTeamGroup(pUserId, pTeamGroupId);
        }

        #endregion

        #region Private
        private int SaveData(User pUser, bool pIsDelete = false)
        {
            int res = 0;
            var user = _context.Users.FirstOrDefault(t => t.Id == pUser.Id);
            if (user != null && pUser.Id != 0 && !pIsDelete)
            {
                user.Email = pUser.Email;
                user.Username = pUser.Username;
                user.Role = pUser.Role;
                user.PasswordHash = pUser.PasswordHash;
            }
            else if (pIsDelete == true && user != null)
            {
                _context.Users.Remove(user);
            }
            else
            {
                _context.Users.Add(pUser);
            }
            res = _context.SaveChanges();
            return res;
        }
        private int SaveData(TeamGroup pTeamGroup, bool pIsDelete = false)
        {
            int res = 0;
            var teamGroup = _context.TeamGroup.FirstOrDefault(t => t.Id == pTeamGroup.Id);
            if (teamGroup != null && pTeamGroup.Id != 0 && !pIsDelete)
            {
                teamGroup.Description = pTeamGroup.Description;
            }
            else if (pIsDelete == true && teamGroup != null)
            {
                var lstuserteam = _context.Users.Where(t => t.TeamGroupId == teamGroup.Id);
                if(lstuserteam != null)
                {
                    foreach(var us in lstuserteam)
                    {
                        us.TeamGroupId = null;
                    }
                    res += _context.SaveChanges();
                }
                var lstproject = _context.Projects.Where(t => t.TeamGroupId == teamGroup.Id);
                if (lstproject != null)
                {
                    foreach (var p in lstproject)
                    {
                        p.TeamGroupId = null;
                    }
                    res += _context.SaveChanges();
                }

                _context.TeamGroup.Remove(teamGroup);
            }
            else
            {
                _context.TeamGroup.Add(pTeamGroup);
                res += _context.SaveChanges();
                res += SaveUsertoTeamGroup(pTeamGroup.CreatedBy ?? 0, pTeamGroup.Id);
            }
            res += _context.SaveChanges();
            return res;
        }
        private int SaveUsertoTeamGroup(int pUserId, int pTeamGroupId)
        {
            int res = 0;
            var user = _context.Users.FirstOrDefault(t => t.Id == pUserId);
            if (user != null && pUserId != 0)
            {
                user.TeamGroupId = pTeamGroupId;
                res = _context.SaveChanges();
            }
            return res;
        }
        #endregion
    }
}

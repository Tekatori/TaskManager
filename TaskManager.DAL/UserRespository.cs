using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DAL.ViewModel;
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
        public List<Users> GetAllUser()
        {
            var us = _context.Users.ToList();
            if (us != null && us.Count() > 0)
                return us;
            return new List<Users>();
        }
        public Users GetUser(int pId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == pId);
            if (user != null) return user;
            return new Users();
        }
        public Users GetUser(string pUserName)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == pUserName);
            if (user != null) return user;
            return new Users();
        }
        public Users GetUserByEmail(string pEmail)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == pEmail);
            if (user != null) return user;
            return new Users();
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
        public int CreateUser(Users pUser)
        {
            pUser.Id = 0;
            return SaveData(pUser);
        }
        public int UpdateUser(Users pUser)
        {
            return SaveData(pUser);
        }
        public int DeleteUser(Users pUser)
        {
            return SaveData(pUser, true);
        }
        public List<Users> GetUserByTeamGroupId(int pTeamGroupId)
        {
            var teamGroup = _context.TeamGroup.FirstOrDefault(t => t.Id == pTeamGroupId);
            if (teamGroup == null || string.IsNullOrWhiteSpace(teamGroup.ListIdUser))
                return new List<Users>();

            var lstIdUser = teamGroup.ListIdUser.ToIntList();
            if (lstIdUser.Count == 0)
                return new List<Users>();

            return _context.Users.Where(x => lstIdUser.Contains(x.Id)).ToList();
        }

        public List<Users> GetUsersExcludingTeamGroupId(int pTeamGroupId)
        {
            var teamGroup = _context.TeamGroup.FirstOrDefault(t => t.Id == pTeamGroupId);
            if (teamGroup == null || string.IsNullOrWhiteSpace(teamGroup.ListIdUser))
                return new List<Users>();

            var lstIdUser = teamGroup.ListIdUser.ToIntList();
            if (lstIdUser.Count == 0)
                return new List<Users>();

            return _context.Users.Where(x => !lstIdUser.Contains(x.Id)).ToList();
        }
        public List<TeamGroup> GetALLTeamGroup()
        {
            var tg = _context.TeamGroup.ToList();
            if (tg != null && tg.Count() > 0)
                return tg;
            return new List<TeamGroup>();
        }
        public List<TeamGroup> GetAllTeamGroupByAccount(int? pIdUser)
        {
            var tg = _context.TeamGroup.Where(t=>t.CreatedBy == pIdUser).ToList();
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
        public int AddUsertoTeamGroup(List<int>? pUserIds, int pTeamGroupId)
        {
            return SaveUsertoTeamGroup(pUserIds, pTeamGroupId);
        }
        public int UpdateRoleUser(int? pIdUser, int? pRole)
        {
            int res = 0;
            if (!pIdUser.HasValue || !pRole.HasValue)
                return res;

            var findus = _context.Users.FirstOrDefault(t => t.Id == pIdUser);
            if (findus != null)
            {
                findus.Role = pRole;
                res = _context.SaveChanges();
            }
            return res;
        }
        public int ChangePassWord(int? pIdUser, string pPasswordNewHash)
        {
            int res = 0;
            if (!pIdUser.HasValue || string.IsNullOrEmpty(pPasswordNewHash))
                return res;

            var us = _context.Users.FirstOrDefault(t => t.Id == pIdUser);

            if (us != null)
            {
                us.PasswordHash = pPasswordNewHash;
                res = _context.SaveChanges();
            }
            return res;
        }

        public Users ForgotPasswordAccount(UserViewModel account)
        {
            if (!string.IsNullOrEmpty(account.Email))
            {
                var us = _context.Users.FirstOrDefault(u => u.Email == account.Email);
                if (us != null)
                {
                    string token = Guid.NewGuid().ToString();
                    us.ResetToken = token;
                    us.ResetTokenExpires = DateTime.Now.AddHours(1);
                    _context.SaveChanges();
                    return us;
                }
            }
            return null;
        }
        public Users ExpTokenResetPasswordAccount(UserViewModel account)
        {
            if (!string.IsNullOrEmpty(account.Token))
            {
                var us = _context.Users.FirstOrDefault(u => u.ResetToken == account.Token && u.ResetTokenExpires > DateTime.Now);
                if (us != null)
                {
                    return us;
                }
            }
            return null;
        }
        public int ResetPasswordAccount(ResetPasswordViewModel model)
        {
            int res = 0;
            if (!string.IsNullOrEmpty(model.Token))
            {
                var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.ResetTokenExpires > DateTime.Now);
                if (user == null)
                {
                    return res;
                }

                user.PasswordHash = model.NewPasswordHash;
                user.ResetToken = null;
                user.ResetTokenExpires = null;
                res = _context.SaveChanges();
            }
            return res;
        }
        #endregion

        #region Private
        private int SaveData(Users pUser, bool pIsDelete = false)
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
                if (pTeamGroup.CreatedBy.HasValue)
                {
                    res += SaveUsertoTeamGroup(new List<int> { pTeamGroup.CreatedBy.Value }, pTeamGroup.Id);
                }
            }
            res += _context.SaveChanges();
            return res;
        }
        private int SaveUsertoTeamGroup(List<int>? pUserIds, int pTeamGroupId)
        {
            if (pUserIds == null || pUserIds.Count == 0)
                return 0;

            var teamGroup = _context.TeamGroup.FirstOrDefault(t => t.Id == pTeamGroupId);
            if (teamGroup == null)
                return 0;

            var oldIds = teamGroup.ListIdUser?.ToIntList() ?? new List<int>();

            var mergedIds = oldIds.Union(pUserIds).Distinct().ToList();

            teamGroup.ListIdUser = string.Join(",", mergedIds);
            return _context.SaveChanges();
        }
  
        #endregion
    }
}

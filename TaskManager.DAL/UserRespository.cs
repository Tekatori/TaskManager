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
            if(user != null) return user;
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
            if(user != null)
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
            return SaveData(pUser,true);
        }

        #endregion

        #region Private
        private int SaveData(User pUser,bool pIsDelete = false)
        {
            int res = 0;
            var user = _context.Users.FirstOrDefault(t=>t.Id == pUser.Id);
            if (user != null && pUser.Id != 0 && !pIsDelete) 
            { 
                user.Email = pUser.Email;
                user.Username = pUser.Username;
                user.Role = pUser.Role;
                user.PasswordHash = pUser.PasswordHash;
            }
            else if(pIsDelete == true && user != null)
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
        #endregion
    }
}

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

        #endregion
    }
}

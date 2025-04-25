using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using TaskManager.BLL;
using TaskManager.Models;
using TaskManager.DAL.ViewModel;
namespace TaskManager.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        IConfiguration _config;
        public UserController(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Login(UserViewModel user)
        {
            if (user == null)
                return Json(new { error = "Dữ liệu không hợp lệ" });
            if (IsTextNull(user.Username))
                return Json(new { error = "Tên đăng nhập không được trống" });
            if (IsTextNull(user.Password))
                return Json(new { error = "Mật khẩu không được trống" });

            User us = new User();
            if(IsEmail(user.Username))
            {
                user.Email = user.Username;
                us = _userService.GetUserByEmail(user.Email);
            }
            else
            {
                us = _userService.GetUser(user.Username);
            }

            if (us != null && us.Id > 0)
            {
                if (VerifyPassword(user.Password, us.PasswordHash))
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, us.Id.ToString()),
                        new Claim(ClaimTypes.Name, us.Username),
                        new Claim(ClaimTypes.Email, us.Email ?? ""),
                        new Claim(ClaimTypes.Role, us.Role?.ToString() ?? "0")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    HttpContext.Session.SetString("username", us.Username);
                    return Json(new { success = true, redirectUrl = "/Home/Index" });
                }
            }
            return Json(new { error = "Tài khoản hoặc mật khẩu không chính xác" });
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Register(UserViewModel user)
        {
            if (user == null)
                return Json(new { error = "Dữ liệu không hợp lệ" });
            if (IsTextNull(user.Email))
                return Json(new { error = "Email không được trống" });
            if (IsTextNull(user.Username))
                return Json(new { error = "Tên đăng nhập không được trống" });
            if (IsTextNull(user.Password))
                return Json(new { error = "Mật khẩu không được trống" });
            if (IsTextNull(user.PasswordConfirm))
                return Json(new { error = "Mật khẩu không được trống" });
            if(user.Password != user.PasswordConfirm)
                return Json(new { error = "Mật khẩu không trùng nhau" });
            if (_userService.isExitUserName(user.Username))
                return Json(new { error = "Tên đăng nhập đã tồn tại" });
            if (_userService.isExitEmailUser(user.Email))
                return Json(new { error = "Email đã tồn tại" });     
            User us = new User();
            us.Email = user.Email;
            us.Username = user.Username;
            us.Role = user.Role;
            us.CreatedAt = DateTime.Now;
            us.PasswordHash = HashPassword(user.Password);
            var res = _userService.CreateUser(us);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { error = "Đăng ký thất bại" });
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete("username");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "User");
        }

        public IActionResult TeamGroup()
        {
            var lstTeamGroup = _userService.GetALLTeamGroup();

            var viewmodel = ExtensionClass.ConvertList<TeamGroup, TeamGroupViewModel>(lstTeamGroup);

            if (viewmodel != null && viewmodel.Count > 0)
            {              
                foreach (var item in viewmodel)
                {
                    var user = _userService.GetUser(item.CreatedBy ?? 0);
                    if (user != null)
                    {
                        item.CreatedByName = user.Username;
                    }
                    item.ListIdUserInt = ExtensionClass.ToIntList(item.ListIdUser);
                }
            }
            var lstUser = _userService.GetAllUser();
            ViewBag.lstUser = lstUser;
            return View(viewmodel);
        }
        [HttpPost]
        public IActionResult CreateTeamGroup(TeamGroup teamGroup)
        {
            if (IsTextNull(teamGroup.Name))
                return Json(new { error = "Tên nhóm không được trống" });
            if (_userService.isExitTeamGroup(teamGroup.Name))
                return Json(new { error = "Tên nhóm đã tồn tại" });
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return Json(new { error = "Người dùng không hợp lệ" });
          
            teamGroup.CreatedAt = DateTime.Now;
            teamGroup.CreatedBy = currentUser.Id;

            var res = _userService.CreateTeamGroup(teamGroup);
            if (res > 0)
            {
                return Json(new { success = true });
            }
            return Json(new { error = "Tạo nhóm thất bại" });
        }
        [HttpPost]
        public IActionResult DeleteTeamGroup(TeamGroup teamGroup)
        {
            if (teamGroup.Id <= 0 || teamGroup == null)
            {
                return Json(new { success = false, error = "Không tìm thấy nhóm" });
            }

            var group = _userService.GetTeamGroup(teamGroup.Id);
            if (group == null)
                return Json(new { success = false, error = "Không tìm thấy nhóm" });

            var res = _userService.DeleteTeamGroup(group);
            if (res > 0)
                return Json(new { success = true });
            return Json(new { success = false, error = "Xoá thất bại" });
        }
        [HttpPost]
        public JsonResult AddUsersToTeam(int teamId, List<int> userIds)
        {
            try
            {
                int res= 0;
                if(userIds != null && userIds.Count > 0)
                {     
                    res = _userService.AddUsertoTeamGroup(userIds, teamId);
                }
                if(res > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Thêm người dùng vào nhóm thất bại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult GetUsersNotForTeam(int teamId)
        {
            try
            {
                var users = _userService.GetUsersExcludingTeamGroupId(teamId); 
                return Json(new { success = true, users = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult GetUsersForTeam(int teamId)
        {
            try
            {
                var users = _userService.GetUserByTeamGroupId(teamId);
                return Json(new { success = true, users = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #region Private
        private bool IsTextNull(string text)
        {
            if(!string.IsNullOrEmpty(text))
                return false;
            return true;
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        private bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }


        #endregion
    }
}

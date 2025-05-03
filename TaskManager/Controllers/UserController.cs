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
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Google;
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

            Users us = new Users();
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
            Users us = new Users();
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserViewModel account)
        {
            var user = _userService.ForgotPasswordAccount(account);

            if (user == null)
            {
                return Json(new { error = "Email không hợp lệ hoặc chưa được đăng ký!" });
            }
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            EmailService _emailService = new EmailService(config);
            string resetLink = Url.Action("ResetPassword", "User", new { Token = user.ResetToken }, Request.Scheme);
            string subject = "🔒 Đặt lại mật khẩu - Teka - Task Manager";
            string htmlContent = _emailService.GetHtmlContentEmail(resetLink);

            _emailService.SendEmail(user.Email, subject, htmlContent);

            return Json(new { success = true, Message= "Hãy kiểm tra email để đặt lại mật khẩu." });
        }
        public IActionResult ConfirmMail(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }
        public ActionResult ResetPassword(UserViewModel account)
        {

            var user = _userService.ExpTokenResetPasswordAccount(account);
            if (user == null)
            {
                ViewBag.TokenExpired = true;
                return View(new ResetPasswordViewModel());
            }
            return View(new ResetPasswordViewModel { Token = account.Token,UserName = user.Username });
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.NewPassword))
                return Json(new { error = "Vui lòng nhập mật khẩu mới." });

            if (string.IsNullOrEmpty(model.ConfirmPassword))
                return Json(new { error = "Vui lòng nhập lại mật khẩu để xác nhận." });

            if (model.NewPassword != model.ConfirmPassword)
                return Json(new { error = "Mật khẩu xác nhận không khớp." });


            model.NewPasswordHash = HashPassword(model.NewPassword);

            var res = _userService.ResetPasswordAccount(model);
            if (res == 0)
            {
                return Json(new { error = "Liên kết đặt lại mật khẩu đã hết hạn. Vui lòng yêu cầu lại bằng cách nhấn 'Bạn quên mật khẩu?' để nhận liên kết mới." });
            }
            return Json(new { success = true});
        }

        #region Login Google
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(email))
                {

                    var finduser = _userService.GetUserByEmail(email);

                    if (finduser != null && finduser.Id > 0)
                    {

                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, finduser.Id.ToString()),
                                new Claim(ClaimTypes.Name, finduser.Username),
                                new Claim(ClaimTypes.Email, finduser.Email ?? ""),
                                new Claim(ClaimTypes.Role, finduser.Role?.ToString() ?? "0")
                            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);


                        HttpContext.Session.SetString("username", finduser.Username);


                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        var newUser = new Users
                        {
                            Email = email,
                            Username = email,  
                            Role = (int)CommonEnums.Role.User,  
                            CreatedAt = DateTime.Now,
                            PasswordHash = HashPassword(Guid.NewGuid().ToString())  
                        };


                        var res = _userService.CreateUser(newUser);

                        if (res > 0)  
                        {
                            var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                                    new Claim(ClaimTypes.Name, newUser.Username),
                                    new Claim(ClaimTypes.Email, newUser.Email ?? ""),
                                    new Claim(ClaimTypes.Role, newUser.Role?.ToString() ?? "0")
                                };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                            HttpContext.Session.SetString("username", newUser.Username);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Login", "User");
                        }
                    }
                }
            }
            return RedirectToAction("Login", "User");
        }

        #endregion

        #region TeamGroup
        public IActionResult TeamGroup()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            if(currentUser.Role == (int)CommonEnums.Role.User)
                return RedirectToAction("Index", "Home");


            var lstTeamGroup = _userService.GetAllTeamGroupByAccount(currentUser.Id);

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
            return View(viewmodel);
        }
        public IActionResult GetListTeamGroup()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            if (currentUser.Role == (int)CommonEnums.Role.User)
                return RedirectToAction("Index", "Home");

            var lstTeamGroup = _userService.GetAllTeamGroupByAccount(currentUser.Id);

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
            return PartialView("_ListTeamGroup", viewmodel);
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
        #endregion

        #region Admin
        public IActionResult RoleUser()
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            if (currentUser.Role != (int)CommonEnums.Role.Admin)
                return RedirectToAction("Index", "Home");

            var models = _userService.GetAllUser();
            int pageIndex = 1;
            int pageSize = 5;
            int totalItems = models.Count;

            var pagedUsers = models
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedResult<Users>
            {
                Items = pagedUsers,
                TotalItems = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };


            return View(result);
        }
        public IActionResult _RoleUser(RoleUserParam param)
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            if (currentUser.Role != (int)CommonEnums.Role.Admin)
                return RedirectToAction("Index", "Home");

            var models = _userService.GetAllUser();

            if (!string.IsNullOrWhiteSpace(param.searchText))
            {
                string keyword = param.searchText.Trim().ToLower();
                models = models
                    .Where(t =>
                        (!string.IsNullOrEmpty(t.Username) && t.Username.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.Email) && t.Email.ToLower().Contains(keyword)))
                    .ToList();
            }

            int totalItems = models.Count;
            var pagedItems = models
                .Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToList();

            var result = new PagedResult<Users>
            {
                Items = pagedItems,
                TotalItems = totalItems,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };


            return PartialView("_ListRoleUser", result);
        }
        [HttpPost]
        public IActionResult UpdateRole(int? Id, int? Role)
        {
            if(!Id.HasValue || !Role.HasValue)
                return Json(new { success = false });

            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null)
                return RedirectToAction("Login", "User");
            if (currentUser.Role != (int)CommonEnums.Role.Admin)
                return Json(new { success = false, message = "Bạn không có quyền cập nhật" });

            var res = _userService.UpdateRoleUser(Id,Role);

            if (res > 0 )
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult ChangePassword(string currentPassword, string newPassword)
        {
            var currentUser = CookieHelper.GetLoggedUser(User);
            if (currentUser == null || !currentUser.Id.HasValue || currentUser.Id == 0)
                return Json(new { success = false, error = "Chưa đăng nhập" });
            if(string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ mật khẩu hiện tại và mật khẩu mới." });
            }    

            var us = _userService.GetUser(currentUser.Id.Value);

            if (us == null)
            {
                return Json(new { success = false, error = "Người dùng không hợp lệ" });
            }

            if (VerifyPassword(currentPassword, us.PasswordHash) == false)
            {
                return Json(new { success = false, error = "Mật khẩu hiện tại không chính xác." });
            }

            string passwordnewhash = HashPassword(newPassword);
            var result = _userService.ChangePassWord(currentUser.Id,passwordnewhash);
            if (result > 0)
                return Json(new { success = true });

            return Json(new { success = false, error = "Thay đổi mật khẩu thất bại" });
        }

        #endregion

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

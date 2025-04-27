using TaskManager.Models;

namespace TaskManager.DAL.ViewModel
{
    public class UserViewModel
    {
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirm { get; set; } = string.Empty;
        public int? Role { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public class TaskParam
    {
        public int? IdProject { get; set; }
        public string textsearch { get; set; } = string.Empty;
        public int? IdStatus { get; set; }
        public int? IdUser { get; set; }
        public int? RoleUser { get; set; } = (int)CommonEnums.Role.User;
    }
    public  class TeamGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string ListIdUser { get; set; } = string.Empty;
        public List<int> ListIdUserInt { get; set; } = new List<int>();
    }
    public class CommentParam
    {
        public int? TaskId { get; set; }

    }
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}

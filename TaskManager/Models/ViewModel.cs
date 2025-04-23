namespace TaskManager.Models
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
        public int IdProject { get; set; } = 0;
        public string textsearch { get; set; } = string.Empty;
    }
    public  class TeamGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }
}

﻿using TaskManager.Models;

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
        public string TaskName { get; set; } = string.Empty;
    }
    public class DashboardViewModel
    {
        public int CountTask { get; set; } = 0;
        public int CountTaskInProcess { get; set; } = 0;
        public int CountTaskDone { get; set; } = 0;
        public int CountProject { get; set; } = 0;
        public int CountUser { get; set; } = 0;
        public List<CommentViewModel> ListLastedComments { get; set; } = new List<CommentViewModel>();
        public List<TaskItem> ListDeadlines { get; set; } = new List<TaskItem>();
        public List<TaskItem> ListRecently  { get; set; } = new List<TaskItem>();
    }
    public class CalendarEvent
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public string ColorClass { get; set; }
        
        public int IdTask { get; set; }
    }
    public class CalendarEventViewModel
    {
        public DateTime StartDate { get; set; } 
        public List<CalendarEvent> Events { get; set; }
    }

    public class CalendarParam
    {
        public int month { get; set; } = 0;
        public int year { get; set; } = 0;
        public int? IdProject { get; set; }
    }
    public class RoleUserParam
    {
        public string searchText { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public string NewPasswordHash { get; set; }
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

}

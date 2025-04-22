using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? Notes { get; set; }

        public int Status { get; set; } = 0; 
        public int Priority { get; set; } = 0; 

        public DateTime? DueDate { get; set; }
        public int? ProjectId { get; set; }
        public int? AssignedTo { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

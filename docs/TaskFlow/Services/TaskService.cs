using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Models
{
    public enum TaskStatus
    {
        Pendiente,
        EnProgreso,
        Completada
    }
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public string Responsible { get; set; }

        [Required]
        public TaskStatus Status { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAT { get; set; }


        public TaskItem(string title, string description, string responsible)
        {
            Title = title;
            Description = description;
            Responsible = responsible;
            CreatedAt = DateTime.Now;
            Status = TaskStatus.Pendiente;
        }
    }
}
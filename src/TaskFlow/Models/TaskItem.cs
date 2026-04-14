using System;

public enum TaskStatus { Pendiente, EnProgreso, Completada }

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Responsible { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pendiente;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
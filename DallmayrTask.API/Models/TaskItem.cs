using DallmayrTask.API.Enums;

namespace DallmayrTask.API.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CustomerId { get; set; }
    public int? MachineId { get; set; }
    public int? TechnicianId { get; set; }
    public TaskType TaskType { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? CompletionNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public Machine? Machine { get; set; }
    public User? Technician { get; set; }
    public ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
    public ICollection<TaskPhoto> Photos { get; set; } = new List<TaskPhoto>();
}

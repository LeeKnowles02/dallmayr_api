using DallmayrTask.API.Enums;

namespace DallmayrTask.API.Models;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public int ChangedByUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Enums.TaskStatus? OldStatus { get; set; }
    public Enums.TaskStatus? NewStatus { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskItem TaskItem { get; set; } = null!;
    public User ChangedBy { get; set; } = null!;
}

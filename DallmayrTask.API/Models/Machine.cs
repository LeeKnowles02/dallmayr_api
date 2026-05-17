namespace DallmayrTask.API.Models;

public class Machine
{
    public int Id { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

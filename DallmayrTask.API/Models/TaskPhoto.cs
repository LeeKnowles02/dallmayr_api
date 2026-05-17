namespace DallmayrTask.API.Models;

public class TaskPhoto
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public int UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public TaskItem TaskItem { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}

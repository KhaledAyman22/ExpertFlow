using TaskManager.Entities.Enums;

namespace TaskManager.Entities;

public class UserTask
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public ICollection<Attachment> Attachments { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<AppUser> Assignees { get; set; }
    public ICollection<Team> Teams { get; set; }
}


namespace TaskManager.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<AppUser> Users { get; set; }
    public ICollection<UserTask> Tasks { get; set; }
}
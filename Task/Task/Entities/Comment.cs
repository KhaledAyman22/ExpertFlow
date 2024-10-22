using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
    
    [ForeignKey(nameof(AppUser))]
    public string UserId { get; set; }
    
    public AppUser AppUser { get; set; }
    
    [ForeignKey(nameof(Task))]
    public Guid TaskId { get; set; }
    
    public UserTask? Task { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Entities;

public class Attachment
{
    public Guid Id { get; set; }
    public byte[] File { get; set; }
    
    [ForeignKey(nameof(Task))]
    public Guid TaskId { get; set; }
 
    public UserTask? Task { get; set; }
}
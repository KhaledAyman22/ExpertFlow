using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Entities;

public class UserLead
{
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public AppUser User { get; set; }
    [ForeignKey(nameof(Lead))]
    public string LeadId { get; set; }
    public AppUser Lead { get; set; }
}
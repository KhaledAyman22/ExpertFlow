using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Entities;

public class AppUser : IdentityUser
{
    [ForeignKey(nameof(Lead))]
    public string? LeadId { get; set; }
    
    public AppUser? Lead { get; set; }
    
    public ICollection<AppUser> ManagedUsers;
    
    public ICollection<Team> Teams { get; set; }
    public ICollection<UserTask> Tasks { get; set; }
    
    public ICollection<IdentityUserClaim<string>> UserClaims { get; set; }
}
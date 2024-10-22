using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Entities.Configurations;

public class AppUserConfig: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.HasMany<Team>(u => u.Teams)
            .WithMany(t => t.Users);
        
        builder.HasMany<UserTask>(u => u.Tasks)
            .WithMany(t => t.Assignees);

        builder.HasMany<IdentityUserClaim<string>>(u => u.UserClaims)
            .WithOne()
            .HasForeignKey("UserId");
        
        builder.HasOne<AppUser>(u => u.Lead)
            .WithMany(u => u.ManagedUsers);
    }   
}

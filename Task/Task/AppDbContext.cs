using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;
using TaskManager.Entities.Configurations;

namespace TaskManager;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<
    AppUser,
    IdentityRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>
>(options)
{
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<UserTask> UserTasks => Set<UserTask>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new AppUserConfig());
        builder.ApplyConfiguration(new AttachmentConfig());
        builder.ApplyConfiguration(new CommentConfig());
        builder.ApplyConfiguration(new TeamConfig());
        builder.ApplyConfiguration(new UserTaskConfig());
    }


}

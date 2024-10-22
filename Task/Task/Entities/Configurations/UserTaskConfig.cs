using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Entities.Configurations;

public class UserTaskConfig : IEntityTypeConfiguration<UserTask>
{
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Description).HasMaxLength(1000).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Priority).IsRequired();
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.DueDate).IsRequired();

        builder.HasMany<Comment>(t => t.Comments)
            .WithOne(c => c.Task);
        builder.HasMany<Team>(t => t.Teams)
            .WithMany(c => c.Tasks);
    }   
}

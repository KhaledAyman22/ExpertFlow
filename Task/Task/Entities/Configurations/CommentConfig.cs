using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Entities.Configurations;

public class CommentConfig: IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Text).HasMaxLength(1000).IsRequired();
        builder.Property(c => c.Date).IsRequired();

        builder.HasOne<AppUser>(c => c.AppUser);
        builder.HasOne<UserTask>(c => c.Task)
            .WithMany(t => t.Comments);
    }   
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Entities.Configurations;

public class TeamConfig: IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.HasMany<AppUser>(t => t.Users)
            .WithMany(u => u.Teams);
        builder.HasMany<UserTask>(t => t.Tasks)
            .WithMany(u => u.Teams);
    }   
}

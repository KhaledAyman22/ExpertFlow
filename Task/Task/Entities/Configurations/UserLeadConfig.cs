using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Entities.Configurations;

public class UserLeadConfig: IEntityTypeConfiguration<UserLead>
{
    public void Configure(EntityTypeBuilder<UserLead> builder)
    {
        builder.HasKey(ul => new { ul.UserId, ul.LeadId });
        builder.Property(ul => ul.LeadId).HasMaxLength(50);
        builder.Property(ul => ul.UserId).HasMaxLength(50);
       
            
    }   
}
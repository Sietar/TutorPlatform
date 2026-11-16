using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorPlatform.Api.Auth;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        
        builder.Property(s => s.UserId).IsRequired();
        builder.HasIndex(s => s.UserId).IsUnique();

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
    }
}
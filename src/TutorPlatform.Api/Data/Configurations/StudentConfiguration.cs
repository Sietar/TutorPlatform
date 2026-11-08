using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
    }
}
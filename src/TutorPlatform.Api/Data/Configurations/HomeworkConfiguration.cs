using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Data.Configurations;

public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
{
    public void Configure(EntityTypeBuilder<Homework> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();
        
        builder.Property(h => h.Status).HasConversion<string>();
        
        builder.Property(h => h.Title).IsRequired().HasMaxLength(200);
        
        builder.Property(h => h.MaterialsUrl).HasMaxLength(2048);
        
        builder.HasOne<Teacher>().WithMany().HasForeignKey(h => h.TeacherId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(h => h.StudentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => new { h.StudentId, h.DueDate });

    }
}
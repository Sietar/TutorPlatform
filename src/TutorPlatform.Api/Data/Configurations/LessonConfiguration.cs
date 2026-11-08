using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Status).HasConversion<string>();
        builder.Ignore(l => l.End);

        builder.HasOne<Teacher>().WithMany().HasForeignKey(l => l.TeacherId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(l => l.StudentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.TeacherId, l.Start });
    }
}
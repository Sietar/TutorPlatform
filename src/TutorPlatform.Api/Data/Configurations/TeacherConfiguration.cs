using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.UserId).IsRequired();
        builder.HasIndex(t => t.UserId).IsUnique();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);

        builder.HasMany(t => t.Students)
            .WithOne(s => s.Teacher)
            .HasForeignKey("TeacherId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(t => t.Students)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
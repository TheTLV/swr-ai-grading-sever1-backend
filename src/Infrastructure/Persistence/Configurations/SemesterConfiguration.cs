using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.semesters trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("semesters");

        builder.HasKey(s => s.SemesterId);
        builder.Property(s => s.SemesterId).HasColumnName("semester_id");

        builder.Property(s => s.SemesterCode).HasColumnName("semester_code").IsRequired().HasMaxLength(50);
        builder.Property(s => s.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(s => s.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(s => s.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(s => s.Status).HasColumnName("status").IsRequired().HasMaxLength(50).HasConversion<string>();

        // Index
        builder.HasIndex(s => s.SemesterCode).IsUnique();
    }
}

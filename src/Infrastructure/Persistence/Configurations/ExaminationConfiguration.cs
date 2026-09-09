using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.examinations trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class ExaminationConfiguration : IEntityTypeConfiguration<Examination>
{
    public void Configure(EntityTypeBuilder<Examination> builder)
    {
        builder.ToTable("examinations");

        builder.HasKey(e => e.ExaminationId);
        builder.Property(e => e.ExaminationId).HasColumnName("examination_id");

        builder.Property(e => e.ExaminationCode).HasColumnName("examination_code").IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
        builder.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(e => e.DurationMinutes).HasColumnName("duration_minutes").IsRequired();
        builder.Property(e => e.BeforeTimeMinutes).HasColumnName("before_time_minutes").IsRequired();
        builder.Property(e => e.Note).HasColumnName("note").HasMaxLength(1000);
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasConversion<int>();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();

        // Foreign key
        builder.HasOne(e => e.Semester)
            .WithMany(s => s.Examinations)
            .HasForeignKey(e => e.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(e => e.ExaminationCode).IsUnique();
        builder.HasIndex(e => e.SemesterId);
    }
}

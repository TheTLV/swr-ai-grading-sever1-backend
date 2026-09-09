using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.submissions trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");

        builder.HasKey(s => s.SubmissionId);
        builder.Property(s => s.SubmissionId).HasColumnName("submission_id");

        builder.Property(s => s.SubmissionName).HasColumnName("submission_name").IsRequired().HasMaxLength(255);
        builder.Property(s => s.Folder).HasColumnName("folder").IsRequired().HasMaxLength(1024);
        builder.Property(s => s.Status).HasColumnName("status").IsRequired().HasConversion<int>();
        builder.Property(s => s.CreatedDate).HasColumnName("created_date").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnName("updated_date").IsRequired();
        builder.Property(s => s.LecturerId).HasColumnName("lecturer_id").IsRequired();

        // Foreign key
        builder.HasOne(s => s.Lecturer)
            .WithMany(u => u.CreatedSubmissions)
            .HasForeignKey(s => s.LecturerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(s => s.LecturerId);
    }
}

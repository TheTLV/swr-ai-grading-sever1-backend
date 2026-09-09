using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.gradings trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class GradingConfiguration : IEntityTypeConfiguration<Grading>
{
    public void Configure(EntityTypeBuilder<Grading> builder)
    {
        builder.ToTable("gradings");

        builder.HasKey(g => g.GradingId);
        builder.Property(g => g.GradingId).HasColumnName("grading_id");

        builder.Property(g => g.GradingCode).HasColumnName("grading_code").IsRequired().HasMaxLength(50);
        builder.Property(g => g.AiScore).HasColumnName("ai_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.LecturerScore).HasColumnName("lecturer_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.AiLogs).HasColumnName("ai_logs");
        builder.Property(g => g.CreateDate).HasColumnName("create_date").IsRequired();
        builder.Property(g => g.UpdateDate).HasColumnName("update_date").IsRequired();
        builder.Property(g => g.Comment).HasColumnName("comment").HasMaxLength(1000);
        builder.Property(g => g.FinalScore).HasColumnName("final_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.Status).HasColumnName("status").IsRequired().HasConversion<int>();
        builder.Property(g => g.SubmissionId).HasColumnName("submission_id").IsRequired();

        // Foreign key
        builder.HasOne(g => g.Submission)
            .WithMany(s => s.Gradings)
            .HasForeignKey(g => g.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(g => g.GradingCode).IsUnique();
        builder.HasIndex(g => g.SubmissionId);
    }
}

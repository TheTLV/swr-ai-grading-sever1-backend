using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.exam_materials trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class ExamMaterialConfiguration : IEntityTypeConfiguration<ExamMaterial>
{
    public void Configure(EntityTypeBuilder<ExamMaterial> builder)
    {
        builder.ToTable("exam_materials");

        builder.HasKey(m => m.ExamMaterialId);
        builder.Property(m => m.ExamMaterialId).HasColumnName("exam_material_id");

        builder.Property(m => m.ExamMaterialCode).HasColumnName("exam_material_code").IsRequired().HasMaxLength(50);
        builder.Property(m => m.FileQuestionDocs).HasColumnName("file_question_docs");
        builder.Property(m => m.FileAnswerRubric).HasColumnName("file_answer_rubric");
        builder.Property(m => m.FileAnswerTemplate).HasColumnName("file_answer_template");
        builder.Property(m => m.CreatedDate).HasColumnName("created_date").IsRequired();
        builder.Property(m => m.UpdatedDate).HasColumnName("updated_date").IsRequired();
        builder.Property(m => m.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(m => m.Status).HasColumnName("status").IsRequired().HasConversion<int>();
        builder.Property(m => m.CreateById).HasColumnName("create_by_id").IsRequired();
        builder.Property(m => m.ExaminationId).HasColumnName("examination_id").IsRequired();

        // Foreign keys
        builder.HasOne(m => m.Examination)
            .WithMany(e => e.ExamMaterials)
            .HasForeignKey(m => m.ExaminationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.CreateBy)
            .WithMany(u => u.CreatedExamMaterials)
            .HasForeignKey(m => m.CreateById)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(m => m.ExamMaterialCode).IsUnique();
        builder.HasIndex(m => m.ExaminationId);
        builder.HasIndex(m => m.CreateById);
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Đề thi / Tài liệu thi (đề thi, đáp án, rubric...).
/// Schema: public.exam_materials — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class ExamMaterial
{
    [Key]
    [Column("exam_material_id")]
    public Guid ExamMaterialId { get; set; }

    [Required]
    [Column("exam_material_code")]
    public string ExamMaterialCode { get; set; } = string.Empty;

    [Column("file_question_docs")]
    public string? FileQuestionDocs { get; set; }

    [Column("file_answer_rubric")]
    public string? FileAnswerRubric { get; set; }

    [Column("file_answer_template")]
    public string? FileAnswerTemplate { get; set; }

    [Required]
    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [Column("updated_date")]
    public DateTime UpdatedDate { get; set; }

    [Required]
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Required]
    [Column("status")]
    public ExamMaterialStatus Status { get; set; }

    [Required]
    [Column("create_by_id")]
    public Guid CreateById { get; set; }

    [Required]
    [Column("examination_id")]
    public Guid ExaminationId { get; set; }

    // Navigation
    [ForeignKey("ExaminationId")]
    public Examination? Examination { get; set; }

    [ForeignKey("CreateById")]
    public User? CreateBy { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Đợt thi / Kỳ thi trong một học kỳ.
/// Schema: public.examinations — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class Examination
{
    [Key]
    [Column("examination_id")]
    public Guid ExaminationId { get; set; }

    [Required]
    [Column("examination_code")]
    public string ExaminationCode { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Required]
    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Column("duration_minutes")]
    public int DurationMinutes { get; set; }

    [Required]
    [Column("before_time_minutes")]
    public int BeforeTimeMinutes { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Required]
    [Column("status")]
    public ExaminationStatus Status { get; set; }

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    // Navigation
    [ForeignKey("SemesterId")]
    public Semester? Semester { get; set; }

    public ICollection<ExamMaterial> ExamMaterials { get; set; } = new List<ExamMaterial>();
}

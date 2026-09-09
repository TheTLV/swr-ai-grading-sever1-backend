using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Bài nộp của sinh viên.
/// Schema: public.submissions — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class Submission
{
    [Key]
    [Column("submission_id")]
    public Guid SubmissionId { get; set; }

    [Required]
    [Column("submission_name")]
    public string SubmissionName { get; set; } = string.Empty;

    [Required]
    [Column("folder")]
    public string Folder { get; set; } = string.Empty;

    [Required]
    [Column("status")]
    public SubmissionStatus Status { get; set; }

    [Required]
    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [Column("updated_date")]
    public DateTime UpdatedDate { get; set; }

    [Required]
    [Column("lecturer_id")]
    public Guid LecturerId { get; set; }

    // Navigation
    [ForeignKey("LecturerId")]
    public User? Lecturer { get; set; }

    public ICollection<Grading> Gradings { get; set; } = new List<Grading>();
}

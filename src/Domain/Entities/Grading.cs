using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Kết quả chấm điểm cho một bài nộp.
/// Schema: public.gradings — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class Grading
{
    [Key]
    [Column("grading_id")]
    public Guid GradingId { get; set; }

    [Required]
    [Column("grading_code")]
    public string GradingCode { get; set; } = string.Empty;

    [Column("ai_score")]
    public decimal? AiScore { get; set; }

    [Column("lecturer_score")]
    public decimal? LecturerScore { get; set; }

    [Column("ai_logs")]
    public string? AiLogs { get; set; }

    [Required]
    [Column("create_date")]
    public DateTime CreateDate { get; set; }

    [Required]
    [Column("update_date")]
    public DateTime UpdateDate { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("final_score")]
    public decimal? FinalScore { get; set; }

    [Required]
    [Column("status")]
    public GradingStatus Status { get; set; }

    [Required]
    [Column("submission_id")]
    public Guid SubmissionId { get; set; }

    // Navigation
    [ForeignKey("SubmissionId")]
    public Submission? Submission { get; set; }
}

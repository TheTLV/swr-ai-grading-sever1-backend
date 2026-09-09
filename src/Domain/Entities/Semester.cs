using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Học kỳ trong hệ thống.
/// Schema: public.semesters — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class Semester
{
    [Key]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("semester_code")]
    public string SemesterCode { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Required]
    [Column("status")]
    public SemesterStatus Status { get; set; }

    // Navigation: examinations
    public ICollection<Examination> Examinations { get; set; } = new List<Examination>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Người dùng trong hệ thống (Student / Lecturer).
/// Schema: public.users — bảng đã có trên Supabase (đã đối chiếu DB-First).
/// </summary>
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Column("birthday")]
    public DateOnly Birthday { get; set; }

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public UserRoleEnum Role { get; set; }

    [Required]
    [Column("cccd")]
    public string CCCD { get; set; } = string.Empty;

    [Required]
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Phân biệt loại user: "Student" hoặc "Lecturer".
    /// </summary>
    [Required]
    [Column("Discriminator")]
    public string Discriminator { get; set; } = string.Empty;

    /// <summary>
    /// Mã giảng viên (chỉ khi Discriminator = "Lecturer").
    /// </summary>
    [Column("lecturer_code")]
    public string? LecturerCode { get; set; }

    /// <summary>
    /// Môn giảng dạy (chỉ khi Discriminator = "Lecturer").
    /// </summary>
    [Column("subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// Mã sinh viên (chỉ khi Discriminator = "Student").
    /// </summary>
    [Column("student_code")]
    public string? StudentCode { get; set; }

    /// <summary>
    /// Ngành học (chỉ khi Discriminator = "Student").
    /// </summary>
    [Column("major")]
    public string? Major { get; set; }

    // Navigation: submissions (bài nộp của sinh viên)
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    // Navigation: submissions (bài nộp được tạo bởi giảng viên)
    public ICollection<Submission> CreatedSubmissions { get; set; } = new List<Submission>();

    // Navigation: exam_materials (đề thi được tạo bởi giảng viên)
    public ICollection<ExamMaterial> CreatedExamMaterials { get; set; } = new List<ExamMaterial>();

    // Navigation: gradings (điểm được chấm bởi giảng viên)
    public ICollection<Grading> Gradings { get; set; } = new List<Grading>();
}

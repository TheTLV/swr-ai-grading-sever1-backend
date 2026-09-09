using Domain.Enums;

namespace Application.DTOs.Examinations;

public class ExaminationDto
{
    public Guid ExaminationId { get; set; }
    public Guid SemesterId { get; set; }
    public string ExaminationCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int BeforeTimeMinutes { get; set; }
    public string? Note { get; set; }
    public ExaminationStatus Status { get; set; }
}

public class CreateExaminationRequest
{
    public Guid SemesterId { get; set; }
    public string ExaminationCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int BeforeTimeMinutes { get; set; }
    public string? Note { get; set; }
    public ExaminationStatus Status { get; set; } = ExaminationStatus.Draft;
}

public class UpdateExaminationRequest
{
    public string? ExaminationCode { get; set; }
    public string? Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public int? BeforeTimeMinutes { get; set; }
    public string? Note { get; set; }
    public ExaminationStatus? Status { get; set; }
}

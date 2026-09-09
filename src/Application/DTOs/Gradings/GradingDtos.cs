using Domain.Enums;

namespace Application.DTOs.Gradings;

public class GradingDto
{
    public Guid GradingId { get; set; }
    public string GradingCode { get; set; } = string.Empty;
    public decimal? AiScore { get; set; }
    public decimal? LecturerScore { get; set; }
    public string? AiLogs { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Comment { get; set; }
    public decimal? FinalScore { get; set; }
    public GradingStatus Status { get; set; }
    public Guid SubmissionId { get; set; }
    public string? SubmissionName { get; set; }
}

public class CreateGradingRequest
{
    public Guid SubmissionId { get; set; }
    public string GradingCode { get; set; } = string.Empty;
    public decimal? AiScore { get; set; }
    public string? Comment { get; set; }
    public GradingStatus Status { get; set; } = GradingStatus.Pending;
}

public class UpdateGradingRequest
{
    public decimal? AiScore { get; set; }
    public decimal? LecturerScore { get; set; }
    public string? Comment { get; set; }
    public decimal? FinalScore { get; set; }
    public GradingStatus? Status { get; set; }
}

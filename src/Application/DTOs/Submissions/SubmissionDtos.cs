using Domain.Enums;

namespace Application.DTOs.Submissions;

public class SubmissionDto
{
    public Guid SubmissionId { get; set; }
    public string SubmissionName { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Guid LecturerId { get; set; }
    public string? LecturerName { get; set; }
}

public class CreateSubmissionRequest
{
    public string SubmissionName { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
}

public class UpdateSubmissionRequest
{
    public string? SubmissionName { get; set; }
    public string? Folder { get; set; }
    public SubmissionStatus? Status { get; set; }
}

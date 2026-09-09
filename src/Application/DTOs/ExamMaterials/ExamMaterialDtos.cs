using Domain.Enums;

namespace Application.DTOs.ExamMaterials;

public class ExamMaterialDto
{
    public Guid ExamMaterialId { get; set; }
    public string ExamMaterialCode { get; set; } = string.Empty;
    public string? FileQuestionDocs { get; set; }
    public string? FileAnswerRubric { get; set; }
    public string? FileAnswerTemplate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public ExamMaterialStatus Status { get; set; }
    public Guid CreateById { get; set; }
    public Guid ExaminationId { get; set; }
    public string? CreateByName { get; set; }
}

public class CreateExamMaterialRequest
{
    public Guid ExaminationId { get; set; }
    public string ExamMaterialCode { get; set; } = string.Empty;
    public string? FileQuestionDocs { get; set; }
    public string? FileAnswerRubric { get; set; }
    public string? FileAnswerTemplate { get; set; }
    public ExamMaterialStatus Status { get; set; } = ExamMaterialStatus.Processing;
}

public class UpdateExamMaterialRequest
{
    public string? ExamMaterialCode { get; set; }
    public string? FileQuestionDocs { get; set; }
    public string? FileAnswerRubric { get; set; }
    public string? FileAnswerTemplate { get; set; }
    public ExamMaterialStatus? Status { get; set; }
}

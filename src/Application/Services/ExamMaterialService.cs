using Application.Common;
using Application.DTOs.ExamMaterials;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ExamMaterialService : IExamMaterialService
{
    private readonly IExamMaterialRepository _materials;
    private readonly IExaminationRepository _exams;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public ExamMaterialService(
        IExamMaterialRepository materials,
        IExaminationRepository exams,
        IUserRepository users,
        IUnitOfWork uow)
    {
        _materials = materials;
        _exams = exams;
        _users = users;
        _uow = uow;
    }

    public async Task<PagedResult<ExamMaterialDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
    {
        var all = await _materials.FindAsync(m => !m.IsDeleted, ct);
        var total = all.Count;
        var items = all
            .OrderByDescending(m => m.UpdatedDate)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(m => ToDto(m, null))
            .ToList();
        return new PagedResult<ExamMaterialDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
    }

    public async Task<Result<ExamMaterialDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _materials.GetByIdAsync(id, ct);
        if (m == null || m.IsDeleted)
            return Result<ExamMaterialDto>.Failure($"ExamMaterial '{id}' không tồn tại.", "MATERIAL_NOT_FOUND");

        var creator = await _users.GetByIdAsync(m.CreateById, ct);
        return Result<ExamMaterialDto>.Success(ToDto(m, creator?.FullName));
    }

    public async Task<Result<ExamMaterialDto>> CreateAsync(CreateExamMaterialRequest req, Guid currentUserId, CancellationToken ct = default)
    {
        if (await _exams.GetByIdAsync(req.ExaminationId, ct) == null)
            return Result<ExamMaterialDto>.Failure("Đợt thi không tồn tại.", "EXAM_NOT_FOUND");

        if (await _materials.IsCodeExistsAsync(req.ExamMaterialCode, ct))
            return Result<ExamMaterialDto>.Failure("Mã tài liệu đã tồn tại.", "MATERIAL_CODE_EXISTS");

        var m = new ExamMaterial
        {
            ExamMaterialId = Guid.NewGuid(),
            ExamMaterialCode = req.ExamMaterialCode,
            FileQuestionDocs = req.FileQuestionDocs,
            FileAnswerRubric = req.FileAnswerRubric,
            FileAnswerTemplate = req.FileAnswerTemplate,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            IsDeleted = false,
            Status = req.Status,
            CreateById = currentUserId,
            ExaminationId = req.ExaminationId
        };
        await _materials.AddAsync(m, ct);
        await _uow.SaveChangesAsync(ct);

        var creator = await _users.GetByIdAsync(currentUserId, ct);
        return Result<ExamMaterialDto>.Success(ToDto(m, creator?.FullName));
    }

    public async Task<Result<ExamMaterialDto>> UpdateAsync(Guid id, UpdateExamMaterialRequest req, CancellationToken ct = default)
    {
        var m = await _materials.GetByIdAsync(id, ct);
        if (m == null || m.IsDeleted)
            return Result<ExamMaterialDto>.Failure($"ExamMaterial '{id}' không tồn tại.", "MATERIAL_NOT_FOUND");

        if (req.ExamMaterialCode != null) m.ExamMaterialCode = req.ExamMaterialCode;
        if (req.FileQuestionDocs != null) m.FileQuestionDocs = req.FileQuestionDocs;
        if (req.FileAnswerRubric != null) m.FileAnswerRubric = req.FileAnswerRubric;
        if (req.FileAnswerTemplate != null) m.FileAnswerTemplate = req.FileAnswerTemplate;
        if (req.Status.HasValue) m.Status = req.Status.Value;
        m.UpdatedDate = DateTime.UtcNow;

        _materials.Update(m);
        await _uow.SaveChangesAsync(ct);

        var creator = await _users.GetByIdAsync(m.CreateById, ct);
        return Result<ExamMaterialDto>.Success(ToDto(m, creator?.FullName));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _materials.GetByIdAsync(id, ct);
        if (m == null || m.IsDeleted)
            return Result.Failure($"ExamMaterial '{id}' không tồn tại.", "MATERIAL_NOT_FOUND");

        m.IsDeleted = true;
        _materials.Update(m);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ExamMaterialDto ToDto(ExamMaterial m, string? createByName) => new()
    {
        ExamMaterialId = m.ExamMaterialId,
        ExamMaterialCode = m.ExamMaterialCode,
        FileQuestionDocs = m.FileQuestionDocs,
        FileAnswerRubric = m.FileAnswerRubric,
        FileAnswerTemplate = m.FileAnswerTemplate,
        CreatedDate = m.CreatedDate,
        UpdatedDate = m.UpdatedDate,
        IsDeleted = m.IsDeleted,
        Status = m.Status,
        CreateById = m.CreateById,
        ExaminationId = m.ExaminationId,
        CreateByName = createByName
    };
}

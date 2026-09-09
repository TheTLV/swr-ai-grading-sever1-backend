using Application.Common;
using Application.DTOs.Examinations;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ExaminationService : IExaminationService
{
    private readonly IExaminationRepository _exams;
    private readonly ISemesterRepository _semesters;
    private readonly IUnitOfWork _uow;

    public ExaminationService(IExaminationRepository exams, ISemesterRepository semesters, IUnitOfWork uow)
    {
        _exams = exams;
        _semesters = semesters;
        _uow = uow;
    }

    public async Task<PagedResult<ExaminationDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
    {
        var all = await _exams.GetAllAsync(ct);
        var total = all.Count;
        var items = all
            .OrderByDescending(e => e.StartDate)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(ToDto)
            .ToList();

        return new PagedResult<ExaminationDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
    }

    public async Task<Result<ExaminationDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _exams.GetByIdAsync(id, ct);
        if (e == null) return Result<ExaminationDto>.Failure($"Examination '{id}' không tồn tại.", "EXAM_NOT_FOUND");
        return Result<ExaminationDto>.Success(ToDto(e));
    }

    public async Task<Result<ExaminationDto>> CreateAsync(CreateExaminationRequest req, CancellationToken ct = default)
    {
        if (await _semesters.GetByIdAsync(req.SemesterId, ct) == null)
            return Result<ExaminationDto>.Failure("Học kỳ không tồn tại.", "SEMESTER_NOT_FOUND");

        if (await _exams.IsCodeExistsAsync(req.ExaminationCode, ct))
            return Result<ExaminationDto>.Failure("Mã đợt thi đã tồn tại.", "EXAM_CODE_EXISTS");

        if (req.DurationMinutes <= 0)
            return Result<ExaminationDto>.Failure("Duration phải > 0.", "INVALID_DURATION");

        var e = new Examination
        {
            ExaminationId = Guid.NewGuid(),
            SemesterId = req.SemesterId,
            ExaminationCode = req.ExaminationCode,
            Name = req.Name,
            StartDate = req.StartDate,
            StartTime = req.StartTime,
            DurationMinutes = req.DurationMinutes,
            BeforeTimeMinutes = req.BeforeTimeMinutes,
            Note = req.Note,
            Status = req.Status
        };
        await _exams.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ExaminationDto>.Success(ToDto(e));
    }

    public async Task<Result<ExaminationDto>> UpdateAsync(Guid id, UpdateExaminationRequest req, CancellationToken ct = default)
    {
        var e = await _exams.GetByIdAsync(id, ct);
        if (e == null) return Result<ExaminationDto>.Failure($"Examination '{id}' không tồn tại.", "EXAM_NOT_FOUND");

        if (req.ExaminationCode != null) e.ExaminationCode = req.ExaminationCode;
        if (req.Name != null) e.Name = req.Name;
        if (req.StartDate.HasValue) e.StartDate = req.StartDate.Value;
        if (req.StartTime.HasValue) e.StartTime = req.StartTime.Value;
        if (req.DurationMinutes.HasValue) e.DurationMinutes = req.DurationMinutes.Value;
        if (req.BeforeTimeMinutes.HasValue) e.BeforeTimeMinutes = req.BeforeTimeMinutes.Value;
        if (req.Note != null) e.Note = req.Note;
        if (req.Status.HasValue) e.Status = req.Status.Value;

        if (e.DurationMinutes <= 0)
            return Result<ExaminationDto>.Failure("Duration phải > 0.", "INVALID_DURATION");

        _exams.Update(e);
        await _uow.SaveChangesAsync(ct);
        return Result<ExaminationDto>.Success(ToDto(e));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _exams.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure($"Examination '{id}' không tồn tại.", "EXAM_NOT_FOUND");

        if (await _exams.HasMaterialsAsync(id, ct))
            return Result.Failure("Không thể xóa đợt thi đang có tài liệu thi.", "EXAM_HAS_MATERIALS");

        _exams.Remove(e);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ExaminationDto ToDto(Examination e) => new()
    {
        ExaminationId = e.ExaminationId,
        SemesterId = e.SemesterId,
        ExaminationCode = e.ExaminationCode,
        Name = e.Name,
        StartDate = e.StartDate,
        StartTime = e.StartTime,
        DurationMinutes = e.DurationMinutes,
        BeforeTimeMinutes = e.BeforeTimeMinutes,
        Note = e.Note,
        Status = e.Status
    };
}

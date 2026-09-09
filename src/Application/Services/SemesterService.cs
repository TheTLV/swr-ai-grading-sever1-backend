//using Application.Common;
//using Application.DTOs.Semesters;
//using Application.Interfaces;
//using Domain.Entities;

//namespace Application.Services;

//public class SemesterService : ISemesterService
//{
//    private readonly ISemesterRepository _repo;
//    private readonly IUnitOfWork _uow;

//    public SemesterService(ISemesterRepository repo, IUnitOfWork uow)
//    {
//        _repo = repo;
//        _uow = uow;
//    }

//    public async Task<PagedResult<SemesterDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
//    {
//        var all = await _repo.GetAllAsync(ct);
//        var total = all.Count;
//        var items = all
//            .OrderByDescending(s => s.StartDate)
//            .Skip((req.Page - 1) * req.PageSize)
//            .Take(req.PageSize)
//            .Select(ToDto)
//            .ToList();

//        return new PagedResult<SemesterDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
//    }

//    public async Task<Result<SemesterDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
//    {
//        var s = await _repo.GetByIdAsync(id, ct);
//        if (s == null) return Result<SemesterDto>.Failure($"Semester '{id}' không tồn tại.", "SEMESTER_NOT_FOUND");
//        return Result<SemesterDto>.Success(ToDto(s));
//    }

//    public async Task<Result<SemesterDto>> CreateAsync(CreateSemesterRequest req, CancellationToken ct = default)
//    {
//        if (await _repo.IsCodeExistsAsync(req.SemesterCode, ct))
//            return Result<SemesterDto>.Failure("Mã học kỳ đã tồn tại.", "SEMESTER_CODE_EXISTS");

//        if (req.EndDate < req.StartDate)
//            return Result<SemesterDto>.Failure("Ngày kết thúc phải sau ngày bắt đầu.", "INVALID_DATE_RANGE");

//        var s = new Semester
//        {
//            SemesterId = Guid.NewGuid(),
//            SemesterCode = req.SemesterCode,
//            Name = req.Name,
//            StartDate = req.StartDate,
//            EndDate = req.EndDate,
//            Status = req.Status
//        };
//        await _repo.AddAsync(s, ct);
//        await _uow.SaveChangesAsync(ct);
//        return Result<SemesterDto>.Success(ToDto(s));
//    }

//    public async Task<Result<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest req, CancellationToken ct = default)
//    {
//        var s = await _repo.GetByIdAsync(id, ct);
//        if (s == null) return Result<SemesterDto>.Failure($"Semester '{id}' không tồn tại.", "SEMESTER_NOT_FOUND");

//        if (req.SemesterCode != null) s.SemesterCode = req.SemesterCode;
//        if (req.Name != null) s.Name = req.Name;
//        if (req.StartDate.HasValue) s.StartDate = req.StartDate.Value;
//        if (req.EndDate.HasValue) s.EndDate = req.EndDate.Value;
//        if (req.Status.HasValue) s.Status = req.Status.Value;

//        if (s.EndDate < s.StartDate)
//            return Result<SemesterDto>.Failure("Ngày kết thúc phải sau ngày bắt đầu.", "INVALID_DATE_RANGE");

//        _repo.Update(s);
//        await _uow.SaveChangesAsync(ct);
//        return Result<SemesterDto>.Success(ToDto(s));
//    }

//    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
//    {
//        var s = await _repo.GetByIdAsync(id, ct);
//        if (s == null) return Result.Failure($"Semester '{id}' không tồn tại.", "SEMESTER_NOT_FOUND");

//        if (await _repo.HasExaminationsAsync(id, ct))
//            return Result.Failure("Không thể xóa học kỳ đang có đợt thi.", "SEMESTER_HAS_EXAMINATIONS");

//        _repo.Remove(s);
//        await _uow.SaveChangesAsync(ct);
//        return Result.Success();
//    }

//    private static SemesterDto ToDto(Semester s) => new()
//    {
//        SemesterId = s.SemesterId,
//        SemesterCode = s.SemesterCode,
//        Name = s.Name,
//        StartDate = s.StartDate,
//        EndDate = s.EndDate,
//        Status = s.Status
//    };
//}

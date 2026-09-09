using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public Task<User?> GetByCCCDAsync(string cccd, CancellationToken ct = default)
        => FirstOrDefaultAsync(u => u.Cccd == cccd && !u.IsDeleted, ct);

    public Task<User?> GetByStudentCodeAsync(string studentCode, CancellationToken ct = default)
        => _db.Set<Student>().AsNoTracking().FirstOrDefaultAsync(u => u.StundentCode == studentCode && !u.IsDeleted, ct);

    public Task<User?> GetByLecturerCodeAsync(string lecturerCode, CancellationToken ct = default)
        => _db.Set<Lecturer>().AsNoTracking().FirstOrDefaultAsync(u => u.LecturerCode == lecturerCode && !u.IsDeleted, ct);

    public Task<bool> IsCCCDExistsAsync(string cccd, CancellationToken ct = default)
        => AnyAsync(u => u.Cccd == cccd && !u.IsDeleted, ct);

    public Task<bool> IsStudentCodeExistsAsync(string studentCode, CancellationToken ct = default)
        => _db.Set<Student>().AnyAsync(u => u.StundentCode == studentCode && !u.IsDeleted, ct);

    public Task<bool> IsLecturerCodeExistsAsync(string lecturerCode, CancellationToken ct = default)
        => _db.Set<Lecturer>().AnyAsync(u => u.LecturerCode == lecturerCode && !u.IsDeleted, ct);
}

public class SemesterRepository : Repository<Semester>, ISemesterRepository
{
    public SemesterRepository(AppDbContext db) : base(db) { }

    public Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        => AnyAsync(s => s.SemesterCode == code, ct);

    public Task<bool> HasExaminationsAsync(Guid semesterId, CancellationToken ct = default)
        => AnyAsync(e => e.SemesterId == semesterId, ct);
}

public class ExaminationRepository : Repository<Examination>, IExaminationRepository
{
    private readonly DbSet<ExamMaterial> _materials;

    public ExaminationRepository(AppDbContext db) : base(db)
    {
        _materials = db.Set<ExamMaterial>();
    }

    public Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        => AnyAsync(e => e.ExaminationCode == code, ct);

    public Task<bool> HasMaterialsAsync(Guid examinationId, CancellationToken ct = default)
        => _materials.AnyAsync(m => m.ExaminationId == examinationId, ct);
}

public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
{
    private readonly DbSet<Grading> _gradings;

    public SubmissionRepository(AppDbContext db) : base(db)
    {
        _gradings = db.Set<Grading>();
    }

    public Task<bool> HasGradingsAsync(Guid submissionId, CancellationToken ct = default)
        => _gradings.AnyAsync(g => g.SubmissionId == submissionId, ct);
}

public class ExamMaterialRepository : Repository<ExamMaterial>, IExamMaterialRepository
{
    public ExamMaterialRepository(AppDbContext db) : base(db) { }

    public Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        => AnyAsync(m => m.ExamMaterialCode == code, ct);
}

public class GradingRepository : Repository<Grading>, IGradingRepository
{
    public GradingRepository(AppDbContext db) : base(db) { }

    public Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        => AnyAsync(g => g.GradingCode == code, ct);
}

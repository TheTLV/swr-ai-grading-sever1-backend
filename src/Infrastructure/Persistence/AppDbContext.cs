using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext chính của hệ thống — mapping với schema public.* trên Supabase.
/// Schema DB đã chốt — code sinh ra từ DB schema thật.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Examination> Examinations => Set<Examination>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<ExamMaterial> ExamMaterials => Set<ExamMaterial>();
    public DbSet<Grading> Gradings => Set<Grading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

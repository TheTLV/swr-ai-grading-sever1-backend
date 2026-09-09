using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

/// <summary>
/// Design-time DbContext factory — dùng cho `dotnet ef` (design-time tools).
/// Tự load .env ở nhiều cấp thư mục để có connection string thật từ Supabase.
/// CHỈ dùng lúc design-time, KHÔNG dùng lúc runtime.
/// </summary>
public class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        LoadEnvFile();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("Infrastructure"));
        return new AppDbContext(builder.Options);
    }

    private static void LoadEnvFile()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", ".env"),
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path)) continue;
            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
                var idx = trimmed.IndexOf('=');
                if (idx <= 0) continue;
                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim().Trim('"');
                Environment.SetEnvironmentVariable(key, value);
            }
            return;
        }
    }
}

using DotNetEnv;

// =========================================================
// Load .env BEFORE CreateBuilder so env vars are available
// when configuration is built.
// =========================================================
// In Development: load .env from project root (one level up from src/4.WebAPI)
// In Production: skip .env, use real secrets manager / CI env vars
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envFile))
{
    Env.Load(envFile);
    Console.WriteLine($"[Startup] Loaded .env from {Path.GetFullPath(envFile)}");
}
else
{
    // Try current dir (for `dotnet run` from project root)
    var altPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(altPath))
    {
        Env.Load(altPath);
        Console.WriteLine($"[Startup] Loaded .env from {Path.GetFullPath(altPath)}");
    }
    else
    {
        Console.WriteLine("[Startup] .env not found. Using OS environment variables only.");
    }
}

// =========================================================
// Build app
// =========================================================
var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Services
// =========================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =========================================================
// Build & run
// =========================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.UseAuthorization();
app.MapControllers();

// Health endpoint — no auth, useful for Supabase + load balancers
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    env = app.Environment.EnvironmentName,
    time = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithTags("Health");

app.Run();

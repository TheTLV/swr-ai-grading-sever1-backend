using System.Text;
using Application.Common;
using Application.Common.Options;
using Application.Interfaces;
using Application.Services;
using DotNetEnv;
using FluentValidation.AspNetCore;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// ============================================================
//  Program.cs — Điểm vào của Backend.Server1
//  Cấu trúc: Clean Architecture 4-layer
//     Domain ← Application ← Infrastructure ← WebApi
//  Schema Supabase là chuẩn — không tự ý sửa database.
// ============================================================

// 1. Load .env TRƯỚC khi đọc bất kỳ config nào
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Đọc connection string từ env
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings__DefaultConnection is not set. Check your .env file.");

// 3. Đăng ký DbContext (chỉ Infrastructure mới reference nó)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
        npgsql.MigrationsAssembly("Infrastructure"));
});

// 4. Unit of Work + Repositories (DIP)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<IExamMaterialRepository, ExamMaterialRepository>();
builder.Services.AddScoped<IGradingRepository, GradingRepository>();

// 4. JWT Authentication + Settings
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException("JWT_SECRET is not set.");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "SwrAiGradingApi";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "SwrAiGradingClient";

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = jwtSecret;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.ExpirationMinutes = 60;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// 5. Đăng ký Application Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IExamMaterialService, ExamMaterialService>();
builder.Services.AddScoped<IGradingService, GradingService>();

// 5.1 Bind AuthOptions từ appsettings (SeedAccounts) + register Seeder
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.AddScoped<IAuthSeeder, AuthSeeder>();

// 6. Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// 7. CORS
var corsOrigins = (Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS") ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    if (corsOrigins.Length > 0)
        p.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    else
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

// 8. Swagger + Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend.Server1", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 10. Seed tài khoản gốc (Admin/Lecturer/Student) khi DB trống — chạy trước pipeline.
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IAuthSeeder>();
    await seeder.SeedAsync();
}

// 9. HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check đơn giản
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    server = "Backend.Server1",
    time = DateTime.UtcNow
}));

app.Run();

using Application.Interfaces;
using Application.Services;
using DotNetEnv;
using FluentValidation.AspNetCore;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Storage;
using Microsoft.OpenApi.Models;
using Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "..",
    "..",
    ".env"
);

Env.Load(Path.GetFullPath(envPath));

// Database
builder.Services.AddDatabase();

// Unit of Work + Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IExamMaterialService, ExamMaterialService>();
builder.Services.AddScoped<IExamMaterialRepository, ExamMaterialRepository>();
builder.Services.AddHttpClient<SupabaseStorage>();

builder.Services.AddScoped<ISupabaseStorage>(sp =>
{
    var inner = sp.GetRequiredService<SupabaseStorage>();
    return new EncryptedSupabaseStorage(inner);
});

// No JWT
// No AuthService
// No AuthSeeder

// Controllers
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// CORS
var corsOrigins = (Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS") ?? "")
    .Split(
        ',',
        StringSplitOptions.RemoveEmptyEntries |
        StringSplitOptions.TrimEntries);

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    if (corsOrigins.Length > 0)
    {
        p.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }
    else
    {
        p.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    }
}));

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Backend.Server1",
            Version = "v1"
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// No authentication/authorization
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    server = "Backend.Server1",
    time = DateTime.UtcNow
}));

app.Run();
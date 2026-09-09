using Domain.Enums;

namespace Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public UserRoleEnum Role { get; set; }
    public string Discriminator { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    // Student fields
    public string? StudentCode { get; set; }
    public string? Major { get; set; }

    // Lecturer fields
    public string? LecturerCode { get; set; }
    public string? Subject { get; set; }
}

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public DateOnly? Birthday { get; set; }
    public bool? IsActive { get; set; }
    public string? Major { get; set; }
    public string? Subject { get; set; }
}

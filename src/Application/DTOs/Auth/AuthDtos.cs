using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.Auth;

public sealed class RegisterRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateOnly Birthday { get; set; }

    [Required, StringLength(20, MinimumLength = 6)]
    [JsonPropertyName("CCCD")]
    public string CCCD { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Range(0, 2)]
    public int Role { get; set; }
}

public sealed class LoginRequest
{
    [Required]
    [JsonPropertyName("CCCD")]
    public string CCCD { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthResponse
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Discriminator { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

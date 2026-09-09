using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

/// <summary>
/// CRUD cho User — chỉ phụ thuộc IUserRepository + IUnitOfWork.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UserService(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
    {
        var users = await _users.FindAsync(u => !u.IsDeleted, ct);
        var total = users.Count;
        var items = users
            .OrderBy(u => u.FullName)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(ToDto)
            .ToList();

        return new PagedResult<UserDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct);
        if (user == null || user.IsDeleted)
            return Result<UserDto>.Failure($"Người dùng với id '{id}' không tồn tại.", "USER_NOT_FOUND");
        return Result<UserDto>.Success(ToDto(user));
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest req, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct);
        if (user == null || user.IsDeleted)
            return Result<UserDto>.Failure($"Người dùng với id '{id}' không tồn tại.", "USER_NOT_FOUND");

        if (req.FullName != null) user.FullName = req.FullName;
        if (req.Birthday.HasValue) user.Birthday = req.Birthday.Value;
        if (req.IsActive.HasValue) user.IsActive = req.IsActive.Value;
        if (req.Major != null) user.Major = req.Major;
        if (req.Subject != null) user.Subject = req.Subject;

        _users.Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result<UserDto>.Success(ToDto(user));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct);
        if (user == null || user.IsDeleted)
            return Result.Failure($"Người dùng với id '{id}' không tồn tại.", "USER_NOT_FOUND");

        user.IsDeleted = true;
        user.IsActive = false;
        _users.Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Birthday = u.Birthday,
        CCCD = u.CCCD,
        Role = u.Role,
        Discriminator = u.Discriminator,
        IsActive = u.IsActive,
        IsDeleted = u.IsDeleted,
        StudentCode = u.StudentCode,
        Major = u.Major,
        LecturerCode = u.LecturerCode,
        Subject = u.Subject
    };
}

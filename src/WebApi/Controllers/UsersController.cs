using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// API quản lý người dùng (sinh viên/giảng viên).
/// Yêu cầu JWT đầy đủ (Authorize).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    /// <summary>Lấy danh sách người dùng (phân trang).</summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequest req, CancellationToken ct)
    {
        var result = await _users.GetPagedAsync(req, ct);
        return Ok(result);
    }

    /// <summary>Lấy thông tin người dùng theo id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _users.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    /// <summary>Cập nhật thông tin người dùng.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        var result = await _users.UpdateAsync(id, req, ct);
        return HandleResult(result);
    }

    /// <summary>Xóa mềm người dùng.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _users.DeleteAsync(id, ct);
        return HandleResult(result);
    }

    private IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Data);
        if (result.ErrorCode?.Contains("NOT_FOUND") == true) return NotFound(new { error = result.Error, code = result.ErrorCode });
        return BadRequest(new { error = result.Error, code = result.ErrorCode });
    }

    private IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return Ok(new { success = true });
        if (result.ErrorCode?.Contains("NOT_FOUND") == true) return NotFound(new { error = result.Error, code = result.ErrorCode });
        return BadRequest(new { error = result.Error, code = result.ErrorCode });
    }
}

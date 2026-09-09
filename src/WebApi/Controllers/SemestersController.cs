using Application.Common;
using Application.DTOs.Semesters;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _svc;

    public SemestersController(ISemesterService svc) { _svc = svc; }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequest req, CancellationToken ct)
    {
        var result = await _svc.GetPagedAsync(req, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _svc.GetByIdAsync(id, ct);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest req, CancellationToken ct)
    {
        var result = await _svc.CreateAsync(req, ct);
        return HandleCreatedResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSemesterRequest req, CancellationToken ct)
    {
        var result = await _svc.UpdateAsync(id, req, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _svc.DeleteAsync(id, ct);
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

    private IActionResult HandleCreatedResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return CreatedAtAction(nameof(GetById), new { id = result.Data!.GetType().GetProperty("SemesterId")?.GetValue(result.Data) }, result.Data);
        return BadRequest(new { error = result.Error, code = result.ErrorCode });
    }
}

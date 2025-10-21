using Microsoft.AspNetCore.Mvc;
using Grades.Application.Interfaces;
using Grades.Application.DTOs;
using Grades.Application.Exceptions;

namespace Grades.Api.Controllers;

[ApiController]
[Route("api/snapshots/{snapshotId:int}/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _service;
    public QuestionsController(IQuestionService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuestionDto>>> GetAll(
        int snapshotId,
        [FromQuery] PaginationDto pagination,
        CancellationToken ct = default)
    {
        var items = await _service.GetAllAsync(snapshotId, pagination, ct);
        return Ok(items);
    }


    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(
        int snapshotId, [FromBody] QuestionCreateDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(snapshotId, dto, ct);

            var location = $"/api/snapshots/{snapshotId}/questions/{created.QuestionId}";
            return Created(location, created);
        }
        catch (ZoneNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TestNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut("{questionId:int}")]
    public async Task<ActionResult<QuestionDto>> Update(
        int snapshotId, int questionId, [FromBody] QuestionUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateAsync(snapshotId, questionId, dto, ct);
            return Ok(updated);
        }
        catch (QuestionNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete("{questionId:int}")]
    public async Task<IActionResult> Delete(
        int snapshotId, int questionId, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(snapshotId, questionId, ct);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}

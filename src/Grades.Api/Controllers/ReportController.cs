// Grades.Api/Controllers/ReportsController.cs
using Microsoft.AspNetCore.Mvc;
using Grades.Application.DTOs.Reports;
using Grades.Application.Exceptions;
using Grades.Application.Interfaces;

namespace Grades.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("/api/snapshots/{snapshotId:int}/student-report")]
    public async Task<ActionResult<StudentReportDto>> GetStudentReport(int snapshotId, CancellationToken ct)
    {
        try
        {
            var report = await _reports.GenerateStudentReportAsync(snapshotId, ct);
            return Ok(report);
        }
        catch (SnapshotHasNoQuestionsException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("principal-report")]
    public async Task<ActionResult<PrincipalReportDto>> GetPrincipalReport(
        [FromBody] PrincipalReportRequest request,
        CancellationToken ct)
    {
        try
        {
            var report = await _reports.GeneratePrincipalReportAsync(request.SnapshotIds, ct);
            return Ok(report);
        }
        catch (SnapshotHasNoQuestionsException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}

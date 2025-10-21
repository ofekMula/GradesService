// Grades.Api/Controllers/ReportsController.cs
using Microsoft.AspNetCore.Mvc;
using Grades.Application.DTOs.Reports;
using Grades.Application.Exceptions;
using Grades.Application.Interfaces;

namespace Grades.Api.Controllers;

[ApiController]
[Route("api/snapshots")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("{snapshotId:int}/reports/student")]
    public async Task<ActionResult<StudentReportDto>> GetStudentReport(int snapshotId, CancellationToken ct)
    {
        try
        {
            var report = await _reports.GenerateStudentReportAsync(snapshotId, ct);
            return Ok(report);
        }
        catch (SnapshotNotExistException ex)
        {
            return NotFound(new { error = ex.Message });
        }

        catch (StudentReportNoZones ex)
        {
            return UnprocessableEntity(new
            {
                error = ex.Message,
                snapshots = (ex as dynamic)?.SnapshotId ?? null
            });
        }

        catch (StudentReportValidationException ex)
        {
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("reports/principal")]
    public async Task<ActionResult<PrincipalReportDto>> GetPrincipalReport(
        [FromBody] PrincipalReportRequest request,
        CancellationToken ct)
    {
        try
        {
            var report = await _reports.GeneratePrincipalReportAsync(request.SnapshotIds, ct);
            return Ok(report);
        }
        catch (SnapshotNotExistException ex)
        {
            return NotFound(new { error = ex.Message });
        }

        catch (PrincipalReportValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        catch (PrincipalReportNoZones ex)
        {
            return UnprocessableEntity(new
            {
                error = ex.Message,
                snapshots = (ex as dynamic)?.SnapshotIds ?? null
            });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}

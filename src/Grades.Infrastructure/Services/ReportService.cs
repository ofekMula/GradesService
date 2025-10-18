using Grades.Application.DTOs.Reports;
using Grades.Application.Exceptions;
using Grades.Application.Interfaces;
using Grades.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Grades.Infrastructure.Services;

public sealed class ReportService(GradesDbContext db) : IReportService
{
    public async Task<StudentReportDto> GenerateStudentReportAsync(int snapshotId, CancellationToken ct = default)
    {
        var zoneScores = await db.Questions
            .AsNoTracking()
            .Where(q => q.SnapshotId == snapshotId)
            .GroupBy(q => q.TestId)
            .Select(g => new ZoneScoreDto(
                g.Key,
                "Test " + g.Key,
                g.Select(x => (double?)x.Score).Average() ?? 0.0
            ))
            .ToListAsync(ct);

        if (zoneScores.Count == 0)
            throw new SnapshotHasNoQuestionsException(snapshotId);

        var top = zoneScores.OrderByDescending(z => z.Score).Take(3).ToList();
        var bottom = zoneScores.OrderBy(z => z.Score).Take(3).ToList();
        var low = zoneScores.Where(z => z.Score < 60).OrderBy(z => z.Score).ToList();

        return new StudentReportDto(
            "Student report",
            DateTime.UtcNow,
            top,
            bottom,
            low
        );
    }

    public async Task<PrincipalReportDto> GeneratePrincipalReportAsync(
        IReadOnlyCollection<int> snapshotIds,
        CancellationToken ct = default)
    {

        // Aggregate across snapshots, per TestId (acting as Zone)
        var perZone = await db.Questions
            .AsNoTracking()
            .Where(q => snapshotIds.Contains(q.SnapshotId))
            .GroupBy(q => q.TestId)
            .Select(g => new ZoneScoreDto(
                g.Key,
                "Test " + g.Key, // replace with join to Zones if you have a Zones table
                g.Select(x => (double?)x.Score).Average() ?? 0.0
            ))
            .ToListAsync(ct);

        if (perZone.Count == 0)
            throw new SnapshotHasNoQuestionsException(snapshotIds);

        var lowest = perZone.OrderBy(z => z.Score).First();

        return new PrincipalReportDto(
            "Principal Report",
            DateTime.UtcNow,
            lowest
        );
    }
}

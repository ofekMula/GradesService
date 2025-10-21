using Grades.Application.DTOs.Reports;
using Grades.Application.Exceptions;
using Grades.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace Grades.Infrastructure.Services;

public sealed class ReportService(GradesDbContext db) : IReportService
{
public async Task<StudentReportDto> GenerateStudentReportAsync(
        int snapshotId, CancellationToken ct = default)
    {
        if (snapshotId == 0)
        throw new StudentReportValidationException("Catalog snapshot (0) cannot be used for Student report.");

        var hasAny = await db.Questions
            .AsNoTracking()
            .AnyAsync(q => q.SnapshotId == snapshotId, ct);

        if (!hasAny)
            throw new SnapshotNotExistException(snapshotId);

        var baseRaw =
            from s  in db.Subjects.AsNoTracking()
            join sz in db.SubjectZones.AsNoTracking()
                on new { s.SnapshotId, s.SubjectId } equals new { sz.SnapshotId, sz.SubjectId }
            join z  in db.Zones.AsNoTracking()
                on new { sz.SnapshotId, sz.ZoneId } equals new { z.SnapshotId, z.ZoneId }
            join zq in db.ZonesQuestions.AsNoTracking()
                on new { z.SnapshotId, z.ZoneId } equals new { zq.SnapshotId, zq.ZoneId }
            join q  in db.Questions.AsNoTracking()
                on new { zq.SnapshotId, zq.QuestionId } equals new { q.SnapshotId, q.QuestionId }
            join t  in db.Tests.AsNoTracking()
                on q.TestId equals t.TestId
            where s.SnapshotId == snapshotId
               && q.SnapshotId == snapshotId
               && z.IsRelevant == true
               && q.IsRelevant == true
               && q.Score != null
            select new { z.ZoneId, z.ZoneName, q.Score };

        var zoneScores = await baseRaw
            .GroupBy(x => new { x.ZoneId, x.ZoneName })
            .Select(g => new ZoneScoreDto(
                g.Key.ZoneId,
                g.Key.ZoneName,
                g.Average(x => (double)x.Score!)
            ))
            .ToListAsync(ct);

        if (zoneScores.Count == 0)
            throw new StudentReportNoZones(snapshotId);

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

    if (snapshotIds.Contains(0))
            throw new PrincipalReportValidationException("Catalog snapshot (0) cannot be used for Principal report.");

    if (snapshotIds is null || snapshotIds.Count == 0)
        throw new PrincipalReportValidationException("At least one snapshotId is required.");

    var existing = await db.Questions.AsNoTracking()
    .Where(q => snapshotIds.Contains(q.SnapshotId))
    .Select(q => q.SnapshotId)
    .Distinct()
    .ToListAsync(ct);

    var missing = snapshotIds.Except(existing).ToArray();

    if (missing.Length > 0)
        throw new SnapshotNotExistException(missing);

    var baseRaw =
        from s  in db.Subjects.AsNoTracking()
        join sz in db.SubjectZones.AsNoTracking()
            on new { s.SnapshotId, s.SubjectId } equals new { sz.SnapshotId, sz.SubjectId }
        join z  in db.Zones.AsNoTracking()
            on new { sz.SnapshotId, sz.ZoneId } equals new { z.SnapshotId, z.ZoneId }
        join zq in db.ZonesQuestions.AsNoTracking()
            on new { z.SnapshotId, z.ZoneId } equals new { zq.SnapshotId, zq.ZoneId }
        join q  in db.Questions.AsNoTracking()
            on new { zq.SnapshotId, zq.QuestionId } equals new { q.SnapshotId, q.QuestionId }
        join t  in db.Tests.AsNoTracking()
            on q.TestId equals t.TestId
        where snapshotIds.Contains(s.SnapshotId)
           && snapshotIds.Contains(q.SnapshotId)
           && z.IsRelevant == true
           && q.IsRelevant == true
           && q.Score != null
        select new
        {
            z.ZoneId,
            z.ZoneName,
            Score = (double)q.Score!
        };

    var perZone = await baseRaw
        .GroupBy(x => new { x.ZoneId, x.ZoneName })
        .Select(g => new ZoneScoreDto(
            g.Key.ZoneId,
            g.Key.ZoneName,
            g.Average(x => x.Score)
        ))
        .ToListAsync(ct);

    if (perZone.Count == 0)
        throw new PrincipalReportNoZones(snapshotIds);

    var lowest = perZone.OrderBy(z => z.Score).First();

    return new PrincipalReportDto(
        "Principal Report",
        DateTime.UtcNow,
        lowest
    );
}
}

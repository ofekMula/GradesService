using Grades.Application.Interfaces;
using Grades.Application.DTOs;
using Grades.Application.Exceptions;
using Grades.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Grades.Infrastructure.Services;

public class QuestionService(GradesDbContext db) : IQuestionService
{
    public async Task<IReadOnlyList<QuestionDto>> GetAllAsync(int snapshotId, PaginationDto pagination, CancellationToken ct = default)
    {
        return await db.Questions.AsNoTracking()
        .Where(q => q.SnapshotId == snapshotId)
        .OrderBy(q => q.QuestionId)
        .Skip(pagination.Skip)
        .Take(pagination.Take)
        .Select(q => new QuestionDto(
            q.QuestionId,
            q.QuestionText,
            q.Score,
            q.IsRelevant,
            q.TestId))
        .ToListAsync(ct);
    }

    public async Task<QuestionDto?> CreateAsync(int snapshotId, QuestionCreateDto dto, CancellationToken ct = default)
    {
        var zoneExists = await db.Zones.AsNoTracking()
            .AnyAsync(z => z.SnapshotId == snapshotId && z.ZoneId == dto.ZoneId, ct);
        if (!zoneExists)
            throw new ZoneNotFoundException(snapshotId, dto.ZoneId);

        var testExists = await db.Tests.AsNoTracking()
            .AnyAsync(t => t.TestId == dto.TestId, ct);
        if (!testExists)
            throw new TestNotFoundException(dto.TestId);

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var nextId = await db.Questions
            .Where(q => q.SnapshotId == snapshotId)
            .Select(q => (int?)q.QuestionId)
            .MaxAsync(ct) ?? 0;

        var question = new Question
        {
            SnapshotId   = snapshotId,
            QuestionId   = nextId + 1,
            QuestionText = dto.QuestionText,
            Score        = dto.Score,
            IsRelevant   = dto.IsRelevant,
            TestId       = dto.TestId
        };

        db.Questions.Add(question);

        var zq = new ZonesQuestion
        {
            SnapshotId = snapshotId,
            ZoneId     = dto.ZoneId,
            QuestionId = question.QuestionId
        };
        db.ZonesQuestions.Add(zq);

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return question.ToDto();
    }

    public async Task<QuestionDto?> UpdateAsync(
        int snapshotId,
        int questionId,
        QuestionUpdateDto dto,
        CancellationToken ct = default)
    {
        var entity = await db.Questions
            .FirstOrDefaultAsync(q => q.SnapshotId == snapshotId && q.QuestionId == questionId, ct)
            ?? throw new QuestionNotFoundException(snapshotId ,questionId);

        if (dto.QuestionText is not null)
            entity.QuestionText = dto.QuestionText;

        if (dto.Score is not null)
            entity.Score = dto.Score;

        if (dto.IsRelevant.HasValue)
            entity.IsRelevant = dto.IsRelevant.Value;

        await db.SaveChangesAsync(ct);

        return entity.ToDto();
    }


    public async Task DeleteAsync(int snapshotId, int questionId, CancellationToken ct = default)
    {
        var exists = await db.Questions.AsNoTracking()
        .AnyAsync(q => q.SnapshotId == snapshotId && q.QuestionId == questionId, ct);

        if (!exists)
            throw new QuestionNotFoundException(snapshotId, questionId);

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        await db.ZonesQuestions
            .Where(zq => zq.SnapshotId == snapshotId && zq.QuestionId == questionId)
            .ExecuteDeleteAsync(ct);

        await db.Questions
            .Where(q => q.SnapshotId == snapshotId && q.QuestionId == questionId)
            .ExecuteDeleteAsync(ct);

        await tx.CommitAsync(ct);

    }

}

using Grades.Application.Interfaces;
using Grades.Application.DTOs;
using Grades.Application.Exceptions;
using Grades.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Grades.Infrastructure.Services;

public class QuestionService(GradesDbContext db) : IQuestionService
{
    public async Task<IReadOnlyList<QuestionDto>> GetAllAsync(int snapshotId, CancellationToken ct = default)
    {
        return await db.Questions
            .AsNoTracking()
            .Where(q => q.SnapshotId == snapshotId)
            .OrderBy(q => q.QuestionId)
            .Select(q => q.ToDto()).Take(5)
            .ToListAsync(ct);
    }

    public async Task<QuestionDto?> CreateAsync(int snapshotId, QuestionCreateDto dto, CancellationToken ct = default)
    {
        var nextId = await db.Questions
            .Where(q => q.SnapshotId == snapshotId)
            .Select(q => (int?)q.QuestionId)
            .MaxAsync(ct) ?? 0;

        var entity = new Question
        {
            SnapshotId   = snapshotId,
            QuestionId   = nextId + 1,
            QuestionText = dto.QuestionText,
            Score        = dto.Score,
            IsRelevant   = dto.IsRelevant,
            TestId       = dto.TestId
        };

        db.Questions.Add(entity);
        await db.SaveChangesAsync(ct);

        return entity.ToDto();
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
    var entity = await db.Questions
            .FirstOrDefaultAsync(q => q.SnapshotId == snapshotId && q.QuestionId == questionId, ct)
            ?? throw new QuestionNotFoundException(snapshotId, questionId);

    db.Questions.Remove(entity);
    await db.SaveChangesAsync(ct);
}

}

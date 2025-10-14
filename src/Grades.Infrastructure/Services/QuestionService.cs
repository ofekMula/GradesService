using Grades.Application.Abstractions;
using Grades.Application.DTOs;
using Grades.Domain.Models;
using Grades.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Grades.Infrastructure.Services;

public class QuestionService(GradesDbContext db) : IQuestionService
{
    public async Task<IReadOnlyList<Question>> GetAllAsync(int snapshotId, CancellationToken ct = default)
    {
        return await db.Questions
            .AsNoTracking()
            .Where(q => q.SnapshotId == snapshotId)
            .OrderBy(q => q.QuestionId)
            .ToListAsync(ct);
    }

    public async Task<Question?> CreateAsync(int snapshotId, QuestionCreateDto dto, CancellationToken ct = default)
    {
        if (dto.Score is < 0) return null; // validation: non-negative; add 0..100 if needed

        // Generate next QuestionId within snapshot (since DB doesn't use identity)
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
        return entity;
    }

    public async Task<bool> UpdateAsync(int snapshotId, int questionId, QuestionUpdateDto dto, CancellationToken ct = default)
    {
        if (dto.Score is < 0) return false;

        var entity = await db.Questions
            .FirstOrDefaultAsync(q => q.SnapshotId == snapshotId && q.QuestionId == questionId, ct);

        if (entity is null) return false;

        // QuestionUpdateDto: string? QuestionText
        if (dto.QuestionText is not null)
        {
            entity.QuestionText = dto.QuestionText;
        }

        if (dto.Score is not null)
        {
            entity.Score = dto.Score;
        }

        if (dto.IsRelevant is not null)
        {
            entity.IsRelevant = (bool)dto.IsRelevant;
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int snapshotId, int questionId, CancellationToken ct = default)
    {
        var entity = await db.Questions
            .FirstOrDefaultAsync(q => q.SnapshotId == snapshotId && q.QuestionId == questionId, ct);

        if (entity is null) return false;

        db.Questions.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

using Grades.Domain.Models;
using Grades.Application.DTOs;

namespace Grades.Application.Abstractions;

public interface IQuestionService
{
    Task<IReadOnlyList<Question>> GetAllAsync(int snapshotId, CancellationToken ct = default);
    Task<Question?> CreateAsync(int snapshotId, QuestionCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int snapshotId, int questionId, QuestionUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int snapshotId, int questionId, CancellationToken ct = default);
}

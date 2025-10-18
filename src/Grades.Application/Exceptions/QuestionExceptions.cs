namespace Grades.Application.Exceptions;

public class QuestionNotFoundException : Exception
{
    public QuestionNotFoundException(int snapshotId, int questionId)
        : base($"Question of Snapshot: {snapshotId} with ID: '{questionId}' was not found.") { }
}

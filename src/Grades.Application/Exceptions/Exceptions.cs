namespace Grades.Application.Exceptions;


public sealed class QuestionNotFoundException : Exception
{
    public QuestionNotFoundException(int snapshotId, int questionId)
        : base($"Question of Snapshot: {snapshotId} with ID: '{questionId}' was not found.") { }
}

public sealed class PrincipalReportValidationException : Exception
{
    public PrincipalReportValidationException(string message)
        : base(message) { }
}



public sealed class SnapshotHasNoQuestionsException : Exception
{
    // Single snapshot
    public SnapshotHasNoQuestionsException(int snapshotId)
        : base($"Snapshot '{snapshotId}' has no questions.") { }

    // Multiple snapshots (assumes at least one id)
    public SnapshotHasNoQuestionsException(IEnumerable<int> snapshotIds)
        : base($"Snapshots [{string.Join(", ", snapshotIds)}] have no questions.") { }

    // Convenience params overload
    public SnapshotHasNoQuestionsException(params int[] snapshotIds)
        : this((IEnumerable<int>)snapshotIds) { }
}

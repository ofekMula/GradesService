public sealed class ZoneNotFoundException : Exception
{
    public ZoneNotFoundException(int snapshotId, int zoneId)
        : base($"Zone of Snapshot: {snapshotId} with ID: '{zoneId}' was not found.") { }
}

public sealed class TestNotFoundException : Exception
{
    public TestNotFoundException(int testId)
        : base($"Test with ID: '{testId}' was not found.") { }
}

public sealed class SnapshotNotExistException : Exception
{
    public SnapshotNotExistException(int snapshotId)
        : base($"Snapshot '{snapshotId}' does not exist or has no questions.") { }

    public SnapshotNotExistException(IEnumerable<int> snapshotIds)
        : base($"Snapshots [{string.Join(", ", snapshotIds)}] do not exist or have no questions.") { }

    public SnapshotNotExistException(params int[] snapshotIds)
        : this((IEnumerable<int>)snapshotIds) { }
}

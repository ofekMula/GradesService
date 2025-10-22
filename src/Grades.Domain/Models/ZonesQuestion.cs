namespace Grades.Domain.Models;


public class ZonesQuestion
{
    public int SnapshotId { get; set; }
    public int ZoneId { get; set; }
    public int QuestionId { get; set; }

    public Zone? Zone { get; set; }
    public Question? Question { get; set; }
}

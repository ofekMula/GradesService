namespace Grades.Domain.Models;

public class Question
{
    public int SnapshotId { get; set; }
    public int QuestionId { get; set; }     // Not identity in your DB
    public string QuestionText { get; set; } = default!;
    public int? Score { get; set; }         // null = unanswered
    public bool IsRelevant { get; set; }
    public int TestId { get; set; }
}

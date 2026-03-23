namespace QuestionService.Entities;

public enum QuestionStatus
{
    PENDING = 1,
    APPROVED = 2,
    ASSIGNED = 3,
    ANSWERED = 4
}

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public QuestionStatus Status { get; set; } = QuestionStatus.PENDING;
    public Guid? ApprovedBy { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

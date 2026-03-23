namespace QuestionService.Entities;

public enum QuestionStatus
{
    PENDING = 1,
    APPROVED = 2,
    ASSIGNED = 3,
    ANSWERED = 4
}

public enum QuestionVisibility
{
    PUBLIC = 1,
    GROUP = 2
}

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AskedBy { get; set; }
    public Guid TopicId { get; set; }
    public Guid SemesterId { get; set; }
    public QuestionVisibility Visibility { get; set; } = QuestionVisibility.PUBLIC;
    public QuestionStatus Status { get; set; } = QuestionStatus.PENDING;
    public Guid? ApprovedBy { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

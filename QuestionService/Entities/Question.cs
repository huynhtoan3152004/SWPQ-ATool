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

public class Semester
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Topic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameVn { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public string ResponsibleBy { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Problems { get; set; } = string.Empty;
    public string Actors { get; set; } = string.Empty;
    public string FunctionalRequirements { get; set; } = string.Empty;
    public string? References { get; set; }
    public Guid SemesterId { get; set; }
    public Guid LecturerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

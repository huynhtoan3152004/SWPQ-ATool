using QuestionService.Entities;

namespace QuestionService.Models;

public record CreateQuestionRequest(string Title, string Content, string Topic);

public record AssignQuestionRequest(Guid TeacherId);

public record QuestionResponse(
    Guid Id,
    string Title,
    string Content,
    Guid StudentId,
    string Topic,
    QuestionStatus Status,
    Guid? ApprovedBy,
    Guid? AssignedTo,
    DateTime CreatedAt);

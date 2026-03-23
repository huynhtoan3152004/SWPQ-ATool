using QuestionService.Entities;

namespace QuestionService.Models;

public record CreateQuestionRequest(
    string Title,
    string Content,
    Guid TopicId,
    Guid SemesterId,
    QuestionVisibility Visibility);

public record AssignQuestionRequest(Guid TeacherId);

public record QuestionResponse(
    Guid Id,
    string Title,
    string Content,
    Guid AskedBy,
    Guid TopicId,
    Guid SemesterId,
    QuestionVisibility Visibility,
    QuestionStatus Status,
    Guid? ApprovedBy,
    Guid? AssignedTo,
    DateTime CreatedAt);

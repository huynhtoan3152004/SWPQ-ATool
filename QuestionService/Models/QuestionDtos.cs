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

public record CreateSemesterRequest(
    string Name,
    int Year,
    int Month);

public record SemesterResponse(
    Guid Id,
    string Name,
    int Year,
    int Month,
    DateTime CreatedAt);

public record CreateTopicRequest(
    string Name,
    Guid SemesterId,
    Guid LecturerId);

public record UpdateTopicRequest(
    string? Name,
    Guid? LecturerId);

public record TopicResponse(
    Guid Id,
    string Name,
    Guid SemesterId,
    Guid LecturerId,
    string SemesterName,
    int Year,
    int Month,
    DateTime CreatedAt);

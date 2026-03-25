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

public record TopicAnswerResponse(
    Guid Id,
    Guid QuestionId,
    Guid TeacherId,
    string Content,
    DateTime CreatedAt);

public record TopicQuestionItemResponse(
    QuestionResponse Question,
    List<TopicAnswerResponse> Answers);

public record TopicQuestionThreadResponse(
    TopicResponse Topic,
    List<TopicQuestionItemResponse> Questions);

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
    string Code,
    string NameEn,
    string NameVn,
    string SubmittedBy,
    string ResponsibleBy,
    string Context,
    string Problems,
    string Actors,
    string FunctionalRequirements,
    string? References,
    Guid SemesterId,
    Guid LecturerId);

public record UpdateTopicRequest(
    string? Name,
    string? NameEn,
    string? NameVn,
    string? SubmittedBy,
    string? ResponsibleBy,
    string? Context,
    string? Problems,
    string? Actors,
    string? FunctionalRequirements,
    string? References,
    Guid? LecturerId);

public record TopicResponse(
    Guid Id,
    string Code,
    string Name,
    string NameEn,
    string NameVn,
    string SubmittedBy,
    string ResponsibleBy,
    string Context,
    string Problems,
    string Actors,
    string FunctionalRequirements,
    string? References,
    Guid SemesterId,
    Guid LecturerId,
    string SemesterName,
    int Year,
    int Month,
    DateTime CreatedAt);

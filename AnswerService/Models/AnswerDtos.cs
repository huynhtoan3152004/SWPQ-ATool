namespace AnswerService.Models;

public record CreateAnswerRequest(Guid QuestionId, string Content);

public record AnswerResponse(
    Guid Id,
    Guid QuestionId,
    Guid TeacherId,
    string Content,
    DateTime CreatedAt);

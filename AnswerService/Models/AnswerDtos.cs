namespace AnswerService.Models;

public record CreateAnswerRequest(Guid QuestionId, string Content);

public record GetAnswersByQuestionIdsRequest(List<Guid> QuestionIds);

public record AnswerResponse(
    Guid Id,
    Guid QuestionId,
    Guid TeacherId,
    string Content,
    DateTime CreatedAt);

public record AnswersByQuestionResponse(
    Guid QuestionId,
    List<AnswerResponse> Answers);

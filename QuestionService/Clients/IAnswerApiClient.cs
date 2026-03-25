using QuestionService.Models;

namespace QuestionService.Clients;

public interface IAnswerApiClient
{
    Task<Dictionary<Guid, List<TopicAnswerResponse>>> GetAnswersByQuestionIdsAsync(
        IReadOnlyCollection<Guid> questionIds,
        string bearerToken,
        CancellationToken cancellationToken = default);
}

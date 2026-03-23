namespace AnswerService.Clients;

public interface IQuestionApiClient
{
    Task MarkAnsweredAsync(Guid questionId, string bearerToken, CancellationToken cancellationToken = default);
}

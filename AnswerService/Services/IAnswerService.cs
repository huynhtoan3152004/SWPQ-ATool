using AnswerService.Models;

namespace AnswerService.Services;

public interface IAnswerService
{
    Task<AnswerResponse> CreateAsync(Guid teacherId, string bearerToken, CreateAnswerRequest request, CancellationToken cancellationToken = default);
    Task<List<AnswerResponse>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
}

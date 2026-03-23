using AnswerService.Clients;
using AnswerService.Entities;
using AnswerService.Models;
using AnswerService.Repositories;

namespace AnswerService.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionApiClient _questionApiClient;

    public AnswerService(IAnswerRepository answerRepository, IQuestionApiClient questionApiClient)
    {
        _answerRepository = answerRepository;
        _questionApiClient = questionApiClient;
    }

    public async Task<AnswerResponse> CreateAsync(Guid teacherId, string bearerToken, CreateAnswerRequest request, CancellationToken cancellationToken = default)
    {
        await _questionApiClient.MarkAnsweredAsync(request.QuestionId, bearerToken, cancellationToken);

        var answer = new Answer
        {
            QuestionId = request.QuestionId,
            TeacherId = teacherId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _answerRepository.AddAsync(answer, cancellationToken);
        return ToResponse(created);
    }

    public async Task<List<AnswerResponse>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var answers = await _answerRepository.GetByQuestionIdAsync(questionId, cancellationToken);
        return answers.Select(ToResponse).ToList();
    }

    private static AnswerResponse ToResponse(Answer answer)
    {
        return new AnswerResponse(
            answer.Id,
            answer.QuestionId,
            answer.TeacherId,
            answer.Content,
            answer.CreatedAt);
    }
}

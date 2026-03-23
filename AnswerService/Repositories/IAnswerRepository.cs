using AnswerService.Entities;

namespace AnswerService.Repositories;

public interface IAnswerRepository
{
    Task<Answer> AddAsync(Answer answer, CancellationToken cancellationToken = default);
    Task<List<Answer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
}

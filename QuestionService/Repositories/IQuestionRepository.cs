using QuestionService.Entities;

namespace QuestionService.Repositories;

public interface IQuestionRepository
{
    Task<Question> AddAsync(Question question, CancellationToken cancellationToken = default);
    Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Question>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

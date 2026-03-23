using AnswerService.Data;
using AnswerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly AnswerDbContext _dbContext;

    public AnswerRepository(AnswerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Answer> AddAsync(Answer answer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Answers.AddAsync(answer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return answer;
    }

    public Task<List<Answer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Answers
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

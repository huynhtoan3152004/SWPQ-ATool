using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.Entities;

namespace QuestionService.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly QuestionDbContext _dbContext;

    public QuestionRepository(QuestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Question> AddAsync(Question question, CancellationToken cancellationToken = default)
    {
        await _dbContext.Questions.AddAsync(question, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return question;
    }

    public Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Questions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Question>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Questions.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

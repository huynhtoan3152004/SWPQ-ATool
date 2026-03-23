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

    public Task<List<Question>> GetAllAsync(Guid? topicId, Guid? semesterId, Guid? assignedTo, QuestionVisibility? visibility, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var query = from question in _dbContext.Questions
                    join semester in _dbContext.Semesters on question.SemesterId equals semester.Id
                    select new { question, semester };

        if (topicId.HasValue)
        {
            query = query.Where(x => x.question.TopicId == topicId.Value);
        }

        if (semesterId.HasValue)
        {
            query = query.Where(x => x.question.SemesterId == semesterId.Value);
        }

        if (assignedTo.HasValue)
        {
            query = query.Where(x => x.question.AssignedTo == assignedTo.Value);
        }

        if (visibility.HasValue)
        {
            query = query.Where(x => x.question.Visibility == visibility.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(x => x.semester.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(x => x.semester.Month == month.Value);
        }

        return query
            .OrderByDescending(x => x.question.CreatedAt)
            .Select(x => x.question)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

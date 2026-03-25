using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.Entities;

namespace QuestionService.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly QuestionDbContext _dbContext;

    public TopicRepository(QuestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Topic> AddAsync(Topic topic, CancellationToken cancellationToken = default)
    {
        await _dbContext.Topics.AddAsync(topic, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return topic;
    }

    public Task<bool> ExistsAsync(Guid semesterId, string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Topics.AnyAsync(
            x => x.SemesterId == semesterId && x.Name.ToLower() == name.ToLower(),
            cancellationToken);
    }

    public Task<bool> ExistsOtherAsync(Guid semesterId, string name, Guid excludeTopicId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Topics.AnyAsync(
            x => x.SemesterId == semesterId && x.Id != excludeTopicId && x.Name.ToLower() == name.ToLower(),
            cancellationToken);
    }

    public Task<Topic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Topic?> GetByTopicAndSemesterAsync(Guid topicId, Guid semesterId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == topicId && x.SemesterId == semesterId, cancellationToken);
    }

    public void Remove(Topic topic)
    {
        _dbContext.Topics.Remove(topic);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<(Topic Topic, Semester Semester)>> GetAllWithSemesterAsync(
        Guid? semesterId,
        Guid? lecturerId,
        int? year,
        int? month,
        string? keyword,
        CancellationToken cancellationToken = default)
    {
        var query = from topic in _dbContext.Topics
                    join semester in _dbContext.Semesters on topic.SemesterId equals semester.Id
                    select new { topic, semester };

        if (semesterId.HasValue)
        {
            query = query.Where(x => x.topic.SemesterId == semesterId.Value);
        }

        if (lecturerId.HasValue)
        {
            query = query.Where(x => x.topic.LecturerId == lecturerId.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(x => x.semester.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(x => x.semester.Month == month.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            query = query.Where(x => x.topic.Name.ToLower().Contains(normalized));
        }

        return query
            .OrderByDescending(x => x.semester.Year)
            .ThenByDescending(x => x.semester.Month)
            .ThenBy(x => x.topic.Name)
            .Select(x => new ValueTuple<Topic, Semester>(x.topic, x.semester))
            .ToListAsync(cancellationToken);
    }
}

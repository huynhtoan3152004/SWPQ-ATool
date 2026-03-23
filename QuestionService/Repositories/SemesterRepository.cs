using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.Entities;

namespace QuestionService.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly QuestionDbContext _dbContext;

    public SemesterRepository(QuestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Semester> AddAsync(Semester semester, CancellationToken cancellationToken = default)
    {
        await _dbContext.Semesters.AddAsync(semester, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return semester;
    }

    public Task<bool> ExistsAsync(string name, int year, int month, CancellationToken cancellationToken = default)
    {
        return _dbContext.Semesters.AnyAsync(
            x => x.Name.ToLower() == name.ToLower() && x.Year == year && x.Month == month,
            cancellationToken);
    }

    public Task<List<Semester>> GetAllAsync(string? name, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Semesters.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var keyword = name.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(keyword));
        }

        if (year.HasValue)
        {
            query = query.Where(x => x.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(x => x.Month == month.Value);
        }

        return query
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Semester?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Semesters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}

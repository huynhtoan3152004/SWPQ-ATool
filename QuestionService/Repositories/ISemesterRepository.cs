using QuestionService.Entities;

namespace QuestionService.Repositories;

public interface ISemesterRepository
{
    Task<Semester> AddAsync(Semester semester, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, int year, int month, CancellationToken cancellationToken = default);
    Task<List<Semester>> GetAllAsync(string? name, int? year, int? month, CancellationToken cancellationToken = default);
    Task<Semester?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

using QuestionService.Entities;

namespace QuestionService.Repositories;

public interface ITopicRepository
{
    Task<Topic> AddAsync(Topic topic, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid semesterId, string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsOtherAsync(Guid semesterId, string name, Guid excludeTopicId, CancellationToken cancellationToken = default);
    Task<Topic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Topic?> GetByTopicAndSemesterAsync(Guid topicId, Guid semesterId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<(Topic Topic, Semester Semester)>> GetAllWithSemesterAsync(
        Guid? semesterId,
        Guid? lecturerId,
        int? year,
        int? month,
        string? keyword,
        CancellationToken cancellationToken = default);
}

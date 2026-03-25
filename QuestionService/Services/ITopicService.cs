using QuestionService.Models;

namespace QuestionService.Services;

public interface ITopicService
{
    Task<TopicResponse> CreateAsync(CreateTopicRequest request, CancellationToken cancellationToken = default);
    Task<List<TopicResponse>> GetAllAsync(Guid? semesterId, Guid? lecturerId, int? year, int? month, string? keyword, CancellationToken cancellationToken = default);
    Task<TopicResponse?> GetByIdAsync(Guid topicId, CancellationToken cancellationToken = default);
    Task<TopicResponse?> UpdateAsync(Guid topicId, Guid callerId, bool canReassignLecturer, UpdateTopicRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid topicId, Guid callerId, bool canDeleteAny, CancellationToken cancellationToken = default);
}

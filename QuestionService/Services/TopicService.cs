using QuestionService.Entities;
using QuestionService.Models;
using QuestionService.Repositories;

namespace QuestionService.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISemesterRepository _semesterRepository;

    public TopicService(ITopicRepository topicRepository, ISemesterRepository semesterRepository)
    {
        _topicRepository = topicRepository;
        _semesterRepository = semesterRepository;
    }

    public async Task<TopicResponse> CreateAsync(CreateTopicRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Topic name is required.");
        }

        if (request.SemesterId == Guid.Empty)
        {
            throw new InvalidOperationException("SemesterId is required.");
        }

        if (request.LecturerId == Guid.Empty)
        {
            throw new InvalidOperationException("LecturerId is required.");
        }

        var semester = await _semesterRepository.GetByIdAsync(request.SemesterId, cancellationToken);
        if (semester is null)
        {
            throw new InvalidOperationException("Semester not found.");
        }

        var normalizedName = request.Name.Trim();
        var isDuplicate = await _topicRepository.ExistsAsync(request.SemesterId, normalizedName, cancellationToken);
        if (isDuplicate)
        {
            throw new InvalidOperationException("Topic already exists in this semester.");
        }

        var topic = new Topic
        {
            Name = normalizedName,
            SemesterId = request.SemesterId,
            LecturerId = request.LecturerId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _topicRepository.AddAsync(topic, cancellationToken);
        return ToResponse(created, semester);
    }

    public async Task<List<TopicResponse>> GetAllAsync(Guid? semesterId, Guid? lecturerId, int? year, int? month, string? keyword, CancellationToken cancellationToken = default)
    {
        var topics = await _topicRepository.GetAllWithSemesterAsync(semesterId, lecturerId, year, month, keyword, cancellationToken);
        return topics.Select(x => ToResponse(x.Topic, x.Semester)).ToList();
    }

    public async Task<TopicResponse?> UpdateAsync(Guid topicId, Guid callerId, bool canReassignLecturer, UpdateTopicRequest request, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
        {
            return null;
        }

        if (!canReassignLecturer && topic.LecturerId != callerId)
        {
            throw new InvalidOperationException("You are not allowed to manage this topic.");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            topic.Name = request.Name.Trim();
        }

        if (request.LecturerId.HasValue)
        {
            if (!canReassignLecturer)
            {
                throw new InvalidOperationException("Only GVHD/ADMIN can reassign topic lecturer.");
            }

            if (request.LecturerId.Value == Guid.Empty)
            {
                throw new InvalidOperationException("LecturerId is invalid.");
            }

            topic.LecturerId = request.LecturerId.Value;
        }

        var duplicate = await _topicRepository.ExistsOtherAsync(topic.SemesterId, topic.Name, topic.Id, cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException("Topic name already exists in this semester.");
        }

        await _topicRepository.SaveChangesAsync(cancellationToken);

        var semester = await _semesterRepository.GetByIdAsync(topic.SemesterId, cancellationToken);
        if (semester is null)
        {
            throw new InvalidOperationException("Semester not found.");
        }

        return ToResponse(topic, semester);
    }

    private static TopicResponse ToResponse(Topic topic, Semester semester)
    {
        return new TopicResponse(
            topic.Id,
            topic.Name,
            topic.SemesterId,
            topic.LecturerId,
            semester.Name,
            semester.Year,
            semester.Month,
            topic.CreatedAt);
    }
}

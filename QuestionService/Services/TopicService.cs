using QuestionService.Entities;
using QuestionService.Models;
using QuestionService.Repositories;

namespace QuestionService.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IQuestionRepository _questionRepository;

    public TopicService(ITopicRepository topicRepository, ISemesterRepository semesterRepository, IQuestionRepository questionRepository)
    {
        _topicRepository = topicRepository;
        _semesterRepository = semesterRepository;
        _questionRepository = questionRepository;
    }

    public async Task<TopicResponse> CreateAsync(CreateTopicRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Topic code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.NameEn) || string.IsNullOrWhiteSpace(request.NameVn))
        {
            throw new InvalidOperationException("Topic Name EN/VN are required.");
        }

        if (string.IsNullOrWhiteSpace(request.SubmittedBy) || string.IsNullOrWhiteSpace(request.ResponsibleBy))
        {
            throw new InvalidOperationException("SubmittedBy and ResponsibleBy are required.");
        }

        if (string.IsNullOrWhiteSpace(request.Context)
            || string.IsNullOrWhiteSpace(request.Problems)
            || string.IsNullOrWhiteSpace(request.Actors)
            || string.IsNullOrWhiteSpace(request.FunctionalRequirements))
        {
            throw new InvalidOperationException("Context, Problems, Actors and FunctionalRequirements are required.");
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

        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        var normalizedNameEn = request.NameEn.Trim();
        var normalizedNameVn = request.NameVn.Trim();
        var isDuplicate = await _topicRepository.ExistsAsync(request.SemesterId, normalizedNameEn, cancellationToken);
        if (isDuplicate)
        {
            throw new InvalidOperationException("Topic already exists in this semester.");
        }

        var duplicateCode = await _topicRepository.ExistsCodeAsync(normalizedCode, cancellationToken);
        if (duplicateCode)
        {
            throw new InvalidOperationException("Topic code already exists.");
        }

        var topic = new Topic
        {
            Code = normalizedCode,
            Name = normalizedNameEn,
            NameEn = normalizedNameEn,
            NameVn = normalizedNameVn,
            SubmittedBy = request.SubmittedBy.Trim(),
            ResponsibleBy = request.ResponsibleBy.Trim(),
            Context = request.Context.Trim(),
            Problems = request.Problems.Trim(),
            Actors = request.Actors.Trim(),
            FunctionalRequirements = request.FunctionalRequirements.Trim(),
            References = string.IsNullOrWhiteSpace(request.References) ? null : request.References.Trim(),
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

    public async Task<TopicResponse?> GetByIdAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
        {
            return null;
        }

        var semester = await _semesterRepository.GetByIdAsync(topic.SemesterId, cancellationToken);
        if (semester is null)
        {
            throw new InvalidOperationException("Semester not found.");
        }

        return ToResponse(topic, semester);
    }

    public async Task<TopicResponse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Code is required.");
        }

        var topic = await _topicRepository.GetByCodeAsync(code.Trim().ToUpperInvariant(), cancellationToken);
        if (topic is null)
        {
            return null;
        }

        var semester = await _semesterRepository.GetByIdAsync(topic.SemesterId, cancellationToken);
        if (semester is null)
        {
            throw new InvalidOperationException("Semester not found.");
        }

        return ToResponse(topic, semester);
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
            topic.NameEn = topic.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.NameEn))
        {
            topic.NameEn = request.NameEn.Trim();
            topic.Name = topic.NameEn;
        }

        if (!string.IsNullOrWhiteSpace(request.NameVn))
        {
            topic.NameVn = request.NameVn.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.SubmittedBy))
        {
            topic.SubmittedBy = request.SubmittedBy.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.ResponsibleBy))
        {
            topic.ResponsibleBy = request.ResponsibleBy.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Context))
        {
            topic.Context = request.Context.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Problems))
        {
            topic.Problems = request.Problems.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Actors))
        {
            topic.Actors = request.Actors.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.FunctionalRequirements))
        {
            topic.FunctionalRequirements = request.FunctionalRequirements.Trim();
        }

        topic.References = string.IsNullOrWhiteSpace(request.References) ? topic.References : request.References.Trim();

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

    public async Task<bool> DeleteAsync(Guid topicId, Guid callerId, bool canDeleteAny, CancellationToken cancellationToken = default)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId, cancellationToken);
        if (topic is null)
        {
            return false;
        }

        if (!canDeleteAny && topic.LecturerId != callerId)
        {
            throw new InvalidOperationException("You are not allowed to delete this topic.");
        }

        var hasQuestions = await _questionRepository.ExistsByTopicIdAsync(topicId, cancellationToken);
        if (hasQuestions)
        {
            throw new InvalidOperationException("Cannot delete topic because it already has questions.");
        }

        _topicRepository.Remove(topic);
        await _topicRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TopicResponse ToResponse(Topic topic, Semester semester)
    {
        return new TopicResponse(
            topic.Id,
            topic.Code,
            topic.Name,
            topic.NameEn,
            topic.NameVn,
            topic.SubmittedBy,
            topic.ResponsibleBy,
            topic.Context,
            topic.Problems,
            topic.Actors,
            topic.FunctionalRequirements,
            topic.References,
            topic.SemesterId,
            topic.LecturerId,
            semester.Name,
            semester.Year,
            semester.Month,
            topic.CreatedAt);
    }
}

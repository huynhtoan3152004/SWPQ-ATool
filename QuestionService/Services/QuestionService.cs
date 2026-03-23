using QuestionService.Entities;
using QuestionService.Models;
using QuestionService.Repositories;

namespace QuestionService.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITopicRepository _topicRepository;

    public QuestionService(IQuestionRepository questionRepository, ITopicRepository topicRepository)
    {
        _questionRepository = questionRepository;
        _topicRepository = topicRepository;
    }

    public async Task<QuestionResponse> CreateAsync(Guid askedBy, CreateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        {
            throw new InvalidOperationException("Title and content are required.");
        }

        if (request.TopicId == Guid.Empty || request.SemesterId == Guid.Empty)
        {
            throw new InvalidOperationException("TopicId and SemesterId are required.");
        }

        var topic = await _topicRepository.GetByTopicAndSemesterAsync(request.TopicId, request.SemesterId, cancellationToken);
        if (topic is null)
        {
            throw new InvalidOperationException("Invalid TopicId/SemesterId combination.");
        }

        var question = new Question
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            TopicId = request.TopicId,
            SemesterId = request.SemesterId,
            AskedBy = askedBy,
            Visibility = request.Visibility,
            AssignedTo = topic.LecturerId,
            Status = QuestionStatus.ASSIGNED,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _questionRepository.AddAsync(question, cancellationToken);
        return ToResponse(created);
    }

    public async Task<List<QuestionResponse>> GetAllAsync(Guid? topicId, Guid? semesterId, Guid? assignedTo, QuestionVisibility? visibility, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var questions = await _questionRepository.GetAllAsync(topicId, semesterId, assignedTo, visibility, year, month, cancellationToken);
        return questions.Select(ToResponse).ToList();
    }

    public async Task<QuestionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(id, cancellationToken);
        return question is null ? null : ToResponse(question);
    }

    public async Task<QuestionResponse?> ApproveAsync(Guid questionId, Guid gvhdId, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return null;
        }

        if (question.Status != QuestionStatus.PENDING)
        {
            throw new InvalidOperationException("Only PENDING question can be approved.");
        }

        question.Status = QuestionStatus.APPROVED;
        question.ApprovedBy = gvhdId;

        await _questionRepository.SaveChangesAsync(cancellationToken);
        return ToResponse(question);
    }

    public async Task<QuestionResponse?> AssignAsync(Guid questionId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return null;
        }

        if (question.Status != QuestionStatus.APPROVED)
        {
            throw new InvalidOperationException("Only APPROVED question can be assigned.");
        }

        question.AssignedTo = teacherId;
        question.Status = QuestionStatus.ASSIGNED;

        await _questionRepository.SaveChangesAsync(cancellationToken);
        return ToResponse(question);
    }

    public async Task<QuestionResponse?> MarkAnsweredAsync(Guid questionId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return null;
        }

        if (question.Status != QuestionStatus.ASSIGNED)
        {
            throw new InvalidOperationException("Only ASSIGNED question can be marked ANSWERED.");
        }

        if (!question.AssignedTo.HasValue || question.AssignedTo.Value != teacherId)
        {
            throw new InvalidOperationException("Only assigned lecturer can mark question as ANSWERED.");
        }

        question.Status = QuestionStatus.ANSWERED;

        await _questionRepository.SaveChangesAsync(cancellationToken);
        return ToResponse(question);
    }

    private static QuestionResponse ToResponse(Question question)
    {
        return new QuestionResponse(
            question.Id,
            question.Title,
            question.Content,
            question.AskedBy,
            question.TopicId,
            question.SemesterId,
            question.Visibility,
            question.Status,
            question.ApprovedBy,
            question.AssignedTo,
            question.CreatedAt);
    }
}

using System.Security.Claims;
using AnswerService.Models;
using AnswerService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Controllers;

[ApiController]
[Route("answers")]
[Authorize]
public class AnswersController : ControllerBase
{
    private readonly IAnswerService _answerService;

    public AnswersController(IAnswerService answerService)
    {
        _answerService = answerService;
    }

    [HttpPost]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> Create([FromBody] CreateAnswerRequest request, CancellationToken cancellationToken)
    {
        var teacherId = GetUserId();
        if (teacherId is null)
        {
            return Unauthorized();
        }

        var authHeader = HttpContext.Request.Headers.Authorization.ToString();
        var bearerToken = authHeader.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

        try
        {
            var response = await _answerService.CreateAsync(teacherId.Value, bearerToken, request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByQuestion([FromQuery] Guid questionId, CancellationToken cancellationToken)
    {
        if (questionId == Guid.Empty)
        {
            return BadRequest(new { message = "questionId is required." });
        }

        var response = await _answerService.GetByQuestionIdAsync(questionId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("by-questions")]
    public async Task<IActionResult> GetByQuestions([FromBody] GetAnswersByQuestionIdsRequest request, CancellationToken cancellationToken)
    {
        if (request.QuestionIds is null || request.QuestionIds.Count == 0)
        {
            return Ok(new List<AnswersByQuestionResponse>());
        }

        var questionIds = request.QuestionIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (questionIds.Count == 0)
        {
            return Ok(new List<AnswersByQuestionResponse>());
        }

        var response = await _answerService.GetByQuestionIdsAsync(questionIds, cancellationToken);
        return Ok(response);
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : null;
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Models;
using QuestionService.Services;

namespace QuestionService.Controllers;

[ApiController]
[Route("questions")]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var studentId = GetUserId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        var response = await _questionService.CreateAsync(studentId.Value, request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _questionService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _questionService.GetByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = "GVHD")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var gvhdId = GetUserId();
        if (gvhdId is null)
        {
            return Unauthorized();
        }

        try
        {
            var response = await _questionService.ApproveAsync(id, gvhdId.Value, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "GVHD")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _questionService.AssignAsync(id, request.TeacherId, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/mark-answered")]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> MarkAnswered(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _questionService.MarkAnsweredAsync(id, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : null;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Models;
using QuestionService.Services;
using System.Security.Claims;

namespace QuestionService.Controllers;

[ApiController]
[Route("topics")]
[Authorize]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpPost]
    [Authorize(Roles = "GVHD,ADMIN")]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _topicService.CreateAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? semesterId,
        [FromQuery] Guid? lecturerId,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        var response = await _topicService.GetAllAsync(semesterId, lecturerId, year, month, keyword, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "TEACHER,GVHD,ADMIN")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTopicRequest request, CancellationToken cancellationToken)
    {
        var callerId = GetUserId();
        if (callerId is null)
        {
            return Unauthorized();
        }

        var canReassignLecturer = User.IsInRole("GVHD") || User.IsInRole("ADMIN");

        try
        {
            var response = await _topicService.UpdateAsync(id, callerId.Value, canReassignLecturer, request, cancellationToken);
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

    [HttpGet("my")]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> GetMyTopics(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var response = await _topicService.GetAllAsync(null, userId, null, null, null, cancellationToken);
        return Ok(response);
    }
}

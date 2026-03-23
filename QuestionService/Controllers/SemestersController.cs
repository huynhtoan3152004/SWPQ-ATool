using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Models;
using QuestionService.Services;

namespace QuestionService.Controllers;

[ApiController]
[Route("semesters")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpPost]
    [Authorize(Roles = "GVHD,ADMIN")]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _semesterService.CreateAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken)
    {
        var response = await _semesterService.GetAllAsync(name, year, month, cancellationToken);
        return Ok(response);
    }
}

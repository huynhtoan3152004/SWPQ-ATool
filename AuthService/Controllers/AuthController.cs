using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(response);
    }

    [HttpGet("students")]
    [Authorize(Roles = "GVHD,ADMIN")]
    public async Task<IActionResult> GetStudents(CancellationToken cancellationToken)
    {
        var students = await _authService.GetStudentsAsync(cancellationToken);
        return Ok(students);
    }

    [HttpGet("teachers")]
    [Authorize(Roles = "GVHD,ADMIN")]
    public async Task<IActionResult> GetTeachers(CancellationToken cancellationToken)
    {
        var teachers = await _authService.GetTeachersAsync(cancellationToken);
        return Ok(teachers);
    }

    [HttpPatch("users/{userId:guid}/role")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ChangeRole(Guid userId, [FromBody] ChangeUserRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService.ChangeUserRoleAsync(userId, request.Role, cancellationToken);
        return user is null ? NotFound(new { message = "User not found." }) : Ok(user);
    }
}

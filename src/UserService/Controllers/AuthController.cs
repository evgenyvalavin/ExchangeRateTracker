using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.RegisterAsync(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.LoginAsync(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Logout()
    {
        var token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Token is required"
            });
        }

        var result = await authService.LogoutAsync(token);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserDto> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        var userNameClaim = User.FindFirst("userName")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userNameClaim))
        {
            return Unauthorized();
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return Ok(new UserDto
            {
                Id = userId,
                Name = userNameClaim,
                CreatedAt = DateTime.UtcNow // This would normally come from the database
            });
        }

        return Unauthorized();
    }
}

using ExchangeRateTracker.ApiGateway.Services;
using ExchangeRateTracker.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateTracker.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ProxyService proxyService, IConfiguration configuration, ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userServiceUrl = configuration["UserService:BaseUrl"] ?? "http://user-service:8080";
        var targetUrl = $"{userServiceUrl}/api/auth/register";

        try
        {
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Post,
                System.Text.Json.JsonSerializer.Serialize(request));

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding register request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userServiceUrl = configuration["UserService:BaseUrl"] ?? "http://user-service:8080";
        var targetUrl = $"{userServiceUrl}/api/auth/login";

        try
        {
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Post,
                System.Text.Json.JsonSerializer.Serialize(request));

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding login request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var userServiceUrl = configuration["UserService:BaseUrl"] ?? "http://user-service:8080";
        var targetUrl = $"{userServiceUrl}/api/auth/logout";

        try
        {
            var token = authHeader.Split(" ").Last();
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Post,
                authToken: token);

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding logout request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var userServiceUrl = configuration["UserService:BaseUrl"] ?? "http://user-service:8080";
        var targetUrl = $"{userServiceUrl}/api/auth/me";

        try
        {
            var token = authHeader.Split(" ").Last();
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Get,
                authToken: token);

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding get current user request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

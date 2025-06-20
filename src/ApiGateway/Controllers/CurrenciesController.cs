using Microsoft.AspNetCore.Mvc;
using TrueCodeTestTask.ApiGateway.Services;
using TrueCodeTestTask.Common.DTOs;

namespace TrueCodeTestTask.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController(ProxyService proxyService, IConfiguration configuration, ILogger<CurrenciesController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllCurrencies()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var financeServiceUrl = configuration["FinanceService:BaseUrl"] ?? "http://finance-service:8080";
        var targetUrl = $"{financeServiceUrl}/api/currencies";

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
            logger.LogError(ex, "Error forwarding get currencies request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteCurrencies()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var financeServiceUrl = configuration["FinanceService:BaseUrl"] ?? "http://finance-service:8080";
        var targetUrl = $"{financeServiceUrl}/api/currencies/favorites";

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
            logger.LogError(ex, "Error forwarding get favorite currencies request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavoriteCurrency([FromBody] AddFavoriteCurrencyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var financeServiceUrl = configuration["FinanceService:BaseUrl"] ?? "http://finance-service:8080";
        var targetUrl = $"{financeServiceUrl}/api/currencies/favorites";

        try
        {
            var token = authHeader.Split(" ").Last();
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Post,
                System.Text.Json.JsonSerializer.Serialize(request), token);

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding add favorite currency request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("favorites/{currencyId}")]
    public async Task<IActionResult> RemoveFavoriteCurrency(int currencyId)
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized(new { message = "Authorization header is required" });
        }

        var financeServiceUrl = configuration["FinanceService:BaseUrl"] ?? "http://finance-service:8080";
        var targetUrl = $"{financeServiceUrl}/api/currencies/favorites/{currencyId}";

        try
        {
            var token = authHeader.Split(" ").Last();
            var response = await proxyService.ForwardRequestAsync(targetUrl, HttpMethod.Delete,
                authToken: token);

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding remove favorite currency request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

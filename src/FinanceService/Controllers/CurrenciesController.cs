using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.FinanceService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrenciesController> _logger;

    public CurrenciesController(ICurrencyService currencyService, ILogger<CurrenciesController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CurrencyResponse>> GetAllCurrencies()
    {
        var result = await _currencyService.GetAllCurrenciesAsync();
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<CurrencyResponse>> GetFavoriteCurrencies()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new CurrencyResponse
            {
                Success = false,
                Message = "Invalid user token"
            });
        }

        var result = await _currencyService.GetUserFavoriteCurrenciesAsync(userId);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("favorites")]
    public async Task<ActionResult> AddFavoriteCurrency([FromBody] AddFavoriteCurrencyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var success = await _currencyService.AddFavoriteCurrencyAsync(userId, request.CurrencyId);
        
        if (success)
        {
            return Ok(new { message = "Currency added to favorites successfully" });
        }

        return BadRequest(new { message = "Failed to add currency to favorites" });
    }

    [HttpDelete("favorites/{currencyId}")]
    public async Task<ActionResult> RemoveFavoriteCurrency(int currencyId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var success = await _currencyService.RemoveFavoriteCurrencyAsync(userId, currencyId);
        
        if (success)
        {
            return Ok(new { message = "Currency removed from favorites successfully" });
        }

        return BadRequest(new { message = "Failed to remove currency from favorites" });
    }
}

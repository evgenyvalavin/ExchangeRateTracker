using Grpc.Core;
using TrueCodeTestTask.Common.Grpc;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.FinanceService.Grpc;

public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly ICurrencyService _currencyService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<CurrencyGrpcService> _logger;

    public CurrencyGrpcService(ICurrencyService currencyService, IJwtService jwtService, ILogger<CurrencyGrpcService> logger)
    {
        _currencyService = currencyService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public override async Task<CurrencyListResponse> GetAllCurrencies(GetAllCurrenciesRequest request, ServerCallContext context)
    {
        try
        {
            // Validate token
            if (!_jwtService.ValidateToken(request.Token))
            {
                return new CurrencyListResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var result = await _currencyService.GetAllCurrenciesAsync();

            var response = new CurrencyListResponse
            {
                Success = result.Success,
                Message = result.Message
            };

            foreach (var currency in result.Currencies)
            {
                response.Currencies.Add(new CurrencyInfo
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    Rate = (double)currency.Rate,
                    UpdatedAt = currency.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC GetAllCurrencies method");
            return new CurrencyListResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }

    public override async Task<CurrencyListResponse> GetUserFavoriteCurrencies(GetUserFavoriteCurrenciesRequest request, ServerCallContext context)
    {
        try
        {
            // Validate token
            if (!_jwtService.ValidateToken(request.Token))
            {
                return new CurrencyListResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var result = await _currencyService.GetUserFavoriteCurrenciesAsync(request.UserId);

            var response = new CurrencyListResponse
            {
                Success = result.Success,
                Message = result.Message
            };

            foreach (var currency in result.Currencies)
            {
                response.Currencies.Add(new CurrencyInfo
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    Rate = (double)currency.Rate,
                    UpdatedAt = currency.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC GetUserFavoriteCurrencies method");
            return new CurrencyListResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }

    public override async Task<FavoriteCurrencyResponse> AddFavoriteCurrency(AddFavoriteCurrencyRequest request, ServerCallContext context)
    {
        try
        {
            // Validate token
            if (!_jwtService.ValidateToken(request.Token))
            {
                return new FavoriteCurrencyResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var success = await _currencyService.AddFavoriteCurrencyAsync(request.UserId, request.CurrencyId);

            return new FavoriteCurrencyResponse
            {
                Success = success,
                Message = success ? "Currency added to favorites successfully" : "Failed to add currency to favorites"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC AddFavoriteCurrency method");
            return new FavoriteCurrencyResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }

    public override async Task<FavoriteCurrencyResponse> RemoveFavoriteCurrency(RemoveFavoriteCurrencyRequest request, ServerCallContext context)
    {
        try
        {
            // Validate token
            if (!_jwtService.ValidateToken(request.Token))
            {
                return new FavoriteCurrencyResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var success = await _currencyService.RemoveFavoriteCurrencyAsync(request.UserId, request.CurrencyId);

            return new FavoriteCurrencyResponse
            {
                Success = success,
                Message = success ? "Currency removed from favorites successfully" : "Failed to remove currency from favorites"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC RemoveFavoriteCurrency method");
            return new FavoriteCurrencyResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }
}

using ExchangeRateTracker.FinanceService.Interfaces;
using Grpc.Core;
using ExchangeRateTracker.Common.Grpc;
using ExchangeRateTracker.Common.Interfaces;

namespace ExchangeRateTracker.FinanceService.Grpc;

public class CurrencyGrpcService(ICurrencyService currencyService, IJwtService jwtService, ILogger<CurrencyGrpcService> logger) : CurrencyService.CurrencyServiceBase
{

    public override async Task<CurrencyListResponse> GetAllCurrencies(GetAllCurrenciesRequest request, ServerCallContext context)
    {
        try
        {
            // Validate token
            if (!jwtService.ValidateToken(request.Token))
            {
                return new CurrencyListResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var result = await currencyService.GetAllCurrenciesAsync();

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
            logger.LogError(ex, "Error in gRPC GetAllCurrencies method");
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
            if (!jwtService.ValidateToken(request.Token))
            {
                return new CurrencyListResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var result = await currencyService.GetUserFavoriteCurrenciesAsync(Guid.Parse(request.UserId));

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
            logger.LogError(ex, "Error in gRPC GetUserFavoriteCurrencies method");
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
            if (!jwtService.ValidateToken(request.Token))
            {
                return new FavoriteCurrencyResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var success = await currencyService.AddFavoriteCurrencyAsync(Guid.Parse(request.UserId), request.CurrencyId);

            return new FavoriteCurrencyResponse
            {
                Success = success,
                Message = success ? "Currency added to favorites successfully" : "Failed to add currency to favorites"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC AddFavoriteCurrency method");
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
            if (!jwtService.ValidateToken(request.Token))
            {
                return new FavoriteCurrencyResponse
                {
                    Success = false,
                    Message = "Invalid token"
                };
            }

            var success = await currencyService.RemoveFavoriteCurrencyAsync(Guid.Parse(request.UserId), request.CurrencyId);

            return new FavoriteCurrencyResponse
            {
                Success = success,
                Message = success ? "Currency removed from favorites successfully" : "Failed to remove currency from favorites"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC RemoveFavoriteCurrency method");
            return new FavoriteCurrencyResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }
}

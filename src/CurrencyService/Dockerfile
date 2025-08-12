FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CurrencyService/ExchangeRateTracker.CurrencyService.csproj", "src/CurrencyService/"]
COPY ["src/Common/ExchangeRateTracker.Common.csproj", "src/Common/"]
RUN dotnet restore "src/CurrencyService/ExchangeRateTracker.CurrencyService.csproj"
COPY . .
WORKDIR "/src/src/CurrencyService"
RUN dotnet build "ExchangeRateTracker.CurrencyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeRateTracker.CurrencyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRateTracker.CurrencyService.dll"]

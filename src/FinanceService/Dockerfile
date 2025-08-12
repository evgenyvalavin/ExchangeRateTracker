FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FinanceService/ExchangeRateTracker.FinanceService.csproj", "src/FinanceService/"]
COPY ["src/Common/ExchangeRateTracker.Common.csproj", "src/Common/"]
RUN dotnet restore "src/FinanceService/ExchangeRateTracker.FinanceService.csproj"
COPY . .
WORKDIR "/src/src/FinanceService"
RUN dotnet build "ExchangeRateTracker.FinanceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeRateTracker.FinanceService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRateTracker.FinanceService.dll"]

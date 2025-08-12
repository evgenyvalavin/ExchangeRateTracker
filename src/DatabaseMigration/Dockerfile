FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DatabaseMigration/ExchangeRateTracker.DatabaseMigration.csproj", "src/DatabaseMigration/"]
COPY ["src/Common/ExchangeRateTracker.Common.csproj", "src/Common/"]
RUN dotnet restore "src/DatabaseMigration/ExchangeRateTracker.DatabaseMigration.csproj"
COPY . .
WORKDIR "/src/src/DatabaseMigration"
RUN dotnet build "ExchangeRateTracker.DatabaseMigration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeRateTracker.DatabaseMigration.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRateTracker.DatabaseMigration.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DatabaseMigration/TrueCodeTestTask.DatabaseMigration.csproj", "src/DatabaseMigration/"]
COPY ["src/Common/TrueCodeTestTask.Common.csproj", "src/Common/"]
RUN dotnet restore "src/DatabaseMigration/TrueCodeTestTask.DatabaseMigration.csproj"
COPY . .
WORKDIR "/src/src/DatabaseMigration"
RUN dotnet build "TrueCodeTestTask.DatabaseMigration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCodeTestTask.DatabaseMigration.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCodeTestTask.DatabaseMigration.dll"]

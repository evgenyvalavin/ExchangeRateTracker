FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FinanceService/TrueCodeTestTask.FinanceService.csproj", "src/FinanceService/"]
COPY ["src/Common/TrueCodeTestTask.Common.csproj", "src/Common/"]
RUN dotnet restore "src/FinanceService/TrueCodeTestTask.FinanceService.csproj"
COPY . .
WORKDIR "/src/src/FinanceService"
RUN dotnet build "TrueCodeTestTask.FinanceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCodeTestTask.FinanceService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCodeTestTask.FinanceService.dll"]

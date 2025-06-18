FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ApiGateway/TrueCodeTestTask.ApiGateway.csproj", "src/ApiGateway/"]
COPY ["src/Common/TrueCodeTestTask.Common.csproj", "src/Common/"]
RUN dotnet restore "src/ApiGateway/TrueCodeTestTask.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway"
RUN dotnet build "TrueCodeTestTask.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCodeTestTask.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCodeTestTask.ApiGateway.dll"]

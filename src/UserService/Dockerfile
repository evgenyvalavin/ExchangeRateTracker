FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/UserService/TrueCodeTestTask.UserService.csproj", "src/UserService/"]
COPY ["src/Common/TrueCodeTestTask.Common.csproj", "src/Common/"]
RUN dotnet restore "src/UserService/TrueCodeTestTask.UserService.csproj"
COPY . .
WORKDIR "/src/src/UserService"
RUN dotnet build "TrueCodeTestTask.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCodeTestTask.UserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCodeTestTask.UserService.dll"]

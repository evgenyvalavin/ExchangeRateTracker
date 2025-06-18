FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CurrencyService/TrueCodeTestTask.CurrencyService.csproj", "src/CurrencyService/"]
COPY ["src/Common/TrueCodeTestTask.Common.csproj", "src/Common/"]
RUN dotnet restore "src/CurrencyService/TrueCodeTestTask.CurrencyService.csproj"
COPY . .
WORKDIR "/src/src/CurrencyService"
RUN dotnet build "TrueCodeTestTask.CurrencyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrueCodeTestTask.CurrencyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCodeTestTask.CurrencyService.dll"]

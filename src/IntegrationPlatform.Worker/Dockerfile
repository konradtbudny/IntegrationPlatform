FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/IntegrationPlatform.Worker/IntegrationPlatform.Worker.csproj", "src/IntegrationPlatform.Worker/"]
COPY ["src/IntegrationPlatform.Application/IntegrationPlatform.Application.csproj", "src/IntegrationPlatform.Application/"]
COPY ["src/IntegrationPlatform.Domain/IntegrationPlatform.Domain.csproj", "src/IntegrationPlatform.Domain/"]
COPY ["src/IntegrationPlatform.Infrastructure/IntegrationPlatform.Infrastructure.csproj", "src/IntegrationPlatform.Infrastructure/"]
COPY ["src/IntegrationPlatform.Contracts/IntegrationPlatform.Contracts.csproj", "src/IntegrationPlatform.Contracts/"]

RUN dotnet restore "src/IntegrationPlatform.Worker/IntegrationPlatform.Worker.csproj"

COPY . .

RUN dotnet build "src/IntegrationPlatform.Worker/IntegrationPlatform.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/IntegrationPlatform.Worker/IntegrationPlatform.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationPlatform.Worker.dll"]

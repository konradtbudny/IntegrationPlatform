FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/IntegrationPlatform.Api/IntegrationPlatform.Api.csproj", "src/IntegrationPlatform.Api/"]
COPY ["src/IntegrationPlatform.Application/IntegrationPlatform.Application.csproj", "src/IntegrationPlatform.Application/"]
COPY ["src/IntegrationPlatform.Domain/IntegrationPlatform.Domain.csproj", "src/IntegrationPlatform.Domain/"]
COPY ["src/IntegrationPlatform.Infrastructure/IntegrationPlatform.Infrastructure.csproj", "src/IntegrationPlatform.Infrastructure/"]
COPY ["src/IntegrationPlatform.Contracts/IntegrationPlatform.Contracts.csproj", "src/IntegrationPlatform.Contracts/"]

RUN dotnet restore "src/IntegrationPlatform.Api/IntegrationPlatform.Api.csproj"

COPY . .

RUN dotnet build "src/IntegrationPlatform.Api/IntegrationPlatform.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/IntegrationPlatform.Api/IntegrationPlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationPlatform.Api.dll"]

# IntegrationPlatform

Platforma integracyjna w .NET 8 oparta o Clean Architecture, wspierająca synchroniczne i asynchroniczne przetwarzanie operacji, retry, obsługę timeoutów, śledzenie statusów oraz background workery.

## Architektura

Projekt oparty jest o **Clean Architecture** z następującymi warstwami:

```
src/
├── IntegrationPlatform.Api            # ASP.NET Core Web API
├── IntegrationPlatform.Worker         # Worker Service (background processing)
├── IntegrationPlatform.Domain         # Encje, enumeracje, interfejsy domenowe
├── IntegrationPlatform.Application    # Serwisy, handlery (wzorzec Strategy), DI
├── IntegrationPlatform.Infrastructure # EF Core + SQLite, repozytoria
└── IntegrationPlatform.Contracts      # Żądania/odpowiedzi DTO

tests/
├── IntegrationPlatform.UnitTests      # Testy jednostkowe (xUnit + Moq + FluentAssertions)
└── IntegrationPlatform.IntegrationTests # Testy integracyjne (WebApplicationFactory)
```

## Funkcjonalności

- ✅ Operacje synchroniczne (`POST /api/operations/sync`)
- ✅ Operacje asynchroniczne (`POST /api/operations/async`)
- ✅ Śledzenie statusów operacji (`GET /api/operations/{id}`)
- ✅ Retry mechanizm (`POST /api/operations/{id}/retry`)
- ✅ Anulowanie operacji (`POST /api/operations/{id}/cancel`)
- ✅ Obsługa timeoutów (konfigurowalny `TimeoutSeconds`)
- ✅ Historia operacji (tabela `OperationHistory`)
- ✅ Background processing (Worker Service polling co 5 sekund)
- ✅ Wzorzec Strategy dla handlerów operacji

## Typy operacji

| Typ | Opis |
|-----|------|
| `DataSynchronization` | Synchronizacja danych |
| `FileImport` | Import plików |
| `ReportGeneration` | Generowanie raportów |
| `WebhookDispatch` | Wysyłka webhooków |

## API Endpoints

| Metoda | URL | Opis |
|--------|-----|------|
| `POST` | `/api/operations/sync` | Wykonaj operację synchronicznie |
| `POST` | `/api/operations/async` | Dodaj operację do kolejki asynchronicznej |
| `GET` | `/api/operations/{id}` | Pobierz status i historię operacji |
| `POST` | `/api/operations/{id}/retry` | Ponów nieudaną operację |
| `POST` | `/api/operations/{id}/cancel` | Anuluj oczekującą operację |

## Uruchomienie

### Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (opcjonalnie)

### Lokalne uruchomienie

1. Klonowanie repozytorium:
```bash
git clone <repo-url>
cd IntegrationPlatform
```

2. Uruchomienie API:
```bash
cd src/IntegrationPlatform.Api
dotnet run
```

API będzie dostępne pod adresem: `http://localhost:5000`  
Swagger UI: `http://localhost:5000` (strona główna)

3. Uruchomienie Worker Service (w osobnym terminalu):
```bash
cd src/IntegrationPlatform.Worker
dotnet run
```

### Uruchomienie z Dockerem

```bash
# Zbuduj i uruchom wszystkie usługi
docker-compose up --build

# API dostępne pod: http://localhost:8080
# Swagger UI: http://localhost:8080
```

### Uruchomienie testów

```bash
# Wszystkie testy
dotnet test

# Tylko testy jednostkowe
dotnet test tests/IntegrationPlatform.UnitTests/

# Tylko testy integracyjne
dotnet test tests/IntegrationPlatform.IntegrationTests/
```

## Przykładowe żądania

### Wykonaj operację synchronicznie
```bash
curl -X POST http://localhost:5000/api/operations/sync \
  -H "Content-Type: application/json" \
  -d '{
    "type": 1,
    "payload": "source=db1&target=db2",
    "maxRetries": 3,
    "timeoutSeconds": 30
  }'
```

### Dodaj operację do kolejki asynchronicznej
```bash
curl -X POST http://localhost:5000/api/operations/async \
  -H "Content-Type: application/json" \
  -d '{
    "type": 2,
    "payload": "file.csv"
  }'
```

### Sprawdź status operacji
```bash
curl http://localhost:5000/api/operations/{id}
```

### Ponów nieudaną operację
```bash
curl -X POST http://localhost:5000/api/operations/{id}/retry
```

## Typy operacji (enum OperationType)

```
1 = DataSynchronization
2 = FileImport
3 = ReportGeneration
4 = WebhookDispatch
```

## Technologie

- **.NET 8**
- **ASP.NET Core Web API**
- **Worker Service** (IHostedService / BackgroundService)
- **Entity Framework Core 8** z **SQLite**
- **Swagger / OpenAPI** (Swashbuckle)
- **Dependency Injection**
- **xUnit** + **Moq** + **FluentAssertions**
- **Docker** + **docker-compose**

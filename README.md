# StackOverflowTag

Aplikacja pobierająca tagi ze Stack Overflow API, obliczająca ich procentowy udział i udostępniająca je przez stronicowane REST API z frontendem Angular.

## Architektura

- **StackOverflow.WebAPI** — .NET 10 REST API (Clean Architecture + CQRS/MediatR)
- **StackOverflow.SPA** — Angular + nginx
- **SQL Server 2022** — baza danych

## Wymagania

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (do uruchomienia aplikacji i testów integracyjnych)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (do uruchomienia testów lokalnie)

## Uruchomienie aplikacji

```bash
docker compose up
```

Przy pierwszym uruchomieniu lub po zmianach w kodzie:

```bash
docker compose up --build
```

### Dostępne adresy po uruchomieniu

| Serwis | URL |
|--------|-----|
| Frontend (SPA) | http://localhost/tags |
| Swagger | http://localhost/swagger |
| SQL Server | `localhost:1433` (user: `sa`, password: `SqlServer2022Pass!`) |

## Endpointy API

| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET | `/api/tags?pageNumber=1&pageSize=25&sortBy=name&sortDirection=asc` | Stronicowana lista tagów |
| POST | `/api/tags/refresh` | Wymusza ponowne pobranie tagów z SO API (204) |

**Parametry sortowania:** `sortBy` = `name` / `count` / `percentage`; `sortDirection` = `asc` / `desc`

## Testy

### Testy jednostkowe (nie wymagają Dockera)

```bash
cd StackOverflow.Web
dotnet test StackOverflow.UnitTests
```

17 testów pokrywających:
- `FetchTagsCommandHandler` — logika warunkowego pobierania, obliczanie procentów
- `RefreshTagsCommandHandler` — wymuszanie pobrania, obliczanie procentów
- `GetTagsQueryHandler` — delegacja do repozytorium, mapowanie AutoMapper
- `TagRepository` — sortowanie po różnych polach i kierunkach

### Testy integracyjne (wymagają Dockera)

```bash
cd StackOverflow.Web
dotnet test StackOverflow.IntegrationTests
```

7 testów pokrywających:
- `GET /api/tags` — paginacja, sortowanie, pusta baza
- `POST /api/tags/refresh` — zwraca 204, podmienia tagi w bazie

**Wymagania:** Docker Desktop musi być uruchomiony. Testy używają [Testcontainers](https://dotnet.testcontainers.org/) — automatycznie startują kontener SQL Server 2022 na czas testów (pierwsze uruchomienie pobiera obraz ~700 MB).

### Wszystkie testy naraz

```bash
cd StackOverflow.Web
dotnet test StackOverflow.Web.slnx
```

## Stack technologiczny

- .NET 10, MediatR, AutoMapper, FluentValidation, EF Core + SQL Server
- Angular, nginx
- xUnit, Moq, FluentAssertions, Testcontainers
- Docker Compose
# ProdSight Copilot Instructions

## Build, run, and test commands

```bash
dotnet restore ProdSight.sln
dotnet build ProdSight.sln
dotnet run --project src/ProdSight.Api/ProdSight.Api.csproj
dotnet run --project src/ProdSight.Frontend/ProdSight.Frontend.csproj
dotnet test ProdSight.sln --no-build
```

- `dotnet build ProdSight.sln` currently builds the tracked solution projects successfully on .NET 10.
- `dotnet test ProdSight.sln --no-build` is the current repo-level test entry point, but the tracked `.csproj` files do not include any dedicated test projects yet.
- There is no repo-level lint command or formatting config checked in. Do not invent one in automation.
- A single-test command is not available until a test project exists. When tests are added, prefer `dotnet test <test-project>.csproj --filter "<FullyQualifiedName|DisplayName>"`.

## High-level architecture

- `src/ProdSight.Api` is the API host. It discovers assemblies matching `ProdSight.Api.Modules.*.dll`, registers all `IModule` implementations, and enables each module from `Modules:{ModuleName}:Enabled` in API configuration.
- Each backend module follows the same split: `Domain` for entities, `Application` for feature handlers and endpoint definitions, `Infrastructure` for EF Core / external integrations, and `Composition` for the `IModule` implementation that wires the module into the host.
- `src/ProdSight.Api.Shared` contains the shared abstractions used across modules, especially module discovery (`IModule`, `ModuleRegistry`), endpoint discovery (`IApiEndpoint`), the lightweight request/handler pattern (`IRequest`, `IRequestHandler`), and shared exception types.
- `src/ProdSight.Api.Shared.DTOs` is the contract boundary between backend and frontend. The Blazor app references these DTOs directly, so API contract changes usually require updating both the feature slice and the frontend broker/UI together.
- `src/ProdSight.Frontend` is a standalone Blazor WebAssembly app. OIDC settings come from the `Local` section in `wwwroot/appsettings*.json`, backend base URL comes from `Backend`, and API access goes through `ApiBroker` with named `HttpClient`s (`Api` and `ApiAuthorized`).
- The main shipped business areas are:
  - `IdentityModule`: Keycloak integration plus an EF Core-backed settings store.
  - `DocumentsModule`: folders, business partners, document upload/storage, and EF Core persistence.
  - `MeasurementModule`: currently a thin placeholder module.
- Cross-cutting host endpoints live outside modules in `src/ProdSight.Api/Endpoints.cs`: `/api/v1/status`, `/api/v1/health`, OpenAPI/Scalar, and Prometheus metrics.

## Key conventions

- New backend HTTP features are usually implemented as a single application slice file containing:
  1. a `Request` record implementing `IRequest<T>`
  2. a `Handler` implementing `IRequestHandler<TRequest, TResult>`
  3. an `Endpoint` implementing `IApiEndpoint`
  
  See `DocumentsModule/Application/Features/*`. The scaffolding script `scripts/create-feature.ps1` generates this shape.
- Shared request/response DTOs live under `src/ProdSight.Api.Shared.DTOs/<Module>/<Area>/<Feature>/...`, and feature code aliases them as `DTOs = ...`.
- Module composition classes are the only place that should register module services and module-specific health checks. The host should stay generic and load modules through `IModule`.
- Authorization is claim-based and module-specific. Backend endpoints use explicit claim requirements such as `identity-module-role` / `documents-module-role`, and the frontend uses the same claims to drive menu visibility and route guards.
- Prefer C# primary constructors for dependency-injected services, handlers, components, and exception types instead of classic constructor declarations.
- For expected domain/application failures, throw `AppException` derivatives (`BadRequestException`, `NotFoundException`, etc.) so `ExceptionHandlingMiddleware` can return the standard `ErrorResponse` JSON shape.
- Each EF Core module owns its own `DbContext`, reads schema/connection settings from its module config section, sets `HasDefaultSchema(...)`, and stores `__EFMigrationsHistory` in that same schema. Use the design-time factory in the infrastructure project for EF migration commands.
- Code, comments, and commits are expected to be in English.

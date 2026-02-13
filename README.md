# ProdSight

ProdSight is a modular application for visualizing production data related to factory floors and the operation of machines/production lines. The project is built using .NET and follows modular architecture principles, with modules typically implemented using Onion Architecture. The identity provider used is Keycloak.

## Project Layout

- `src/ProdSight.Api/` — Main API application (endpoints, middleware, module registration).
- `src/ProdSight.Api.Modules/` — Modules (e.g. `IdentityModule`, `MeasurementModule`).
- `src/ProdSight.Api.Shared/` — Shared utilities and interfaces.
- `src/ProdSight.Api.Shared.DTOs/` — DTOs used by the API.
- `src/ProdSight.Frontend/` — Frontend (Blazor WebAssembly).
- `.github/` — CI/CD and AI collaboration instructions.
- `scripts/` — helper scripts (e.g. `build-docker.sh`).

## Build & Publish (local)

Restore, build and publish the API project:

```bash
cd src/ProdSight.Api
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

Note: the project contains an MSBuild `Target` that runs after `Publish` and reorganizes module assemblies into `publish/modules/<ModuleName>`.

## Docker

- Dockerfile: `src/ProdSight.Api/Dockerfile` (multi-stage build: SDK -> publish -> runtime).
- Docker build script: `scripts/build-docker.sh` — builds the image and tags it. When no explicit image tag is provided the script derives a tag from Git (exact tag on HEAD → nearest tag → `sha-<short>`), and the script also tags the image as `:latest`.
- `.dockerignore` is present at repository root to reduce Docker build context (ignores `publish/`, `bin/`, `obj/`, `.git/`, etc.).

Build example (using provided script):

```bash
# default: uses git-derived tag
scripts/build-docker.sh

# or force a specific image name/tag
scripts/build-docker.sh myrepo/prodsight-api:1.2.3
```

## Notes about publish layout and runtime

- The repository includes an MSBuild `Target` named `MoveModuleAssembliesToSubfolder` which, after `dotnet publish`, moves module assemblies into `publish/modules/<ModuleName>`.
- If the application relies on .NET default probing for assemblies (i.e., assemblies present in the application root), moving assemblies out of the root may require the application to explicitly load assemblies from the `modules/` folder at runtime. If your app already supports loading modules from subfolders, the MSBuild target is fine. Otherwise keep assemblies in publish root or implement a loader.
- The `ProdSight.Api.csproj` includes `_ContentIncludedByDefault Remove="publish\..."` entries. These do not prevent `dotnet publish` from generating those files; they instruct MSBuild not to treat existing files under `publish/` as source `Content` of the project (useful to avoid accidentally including local publish artifacts as project content during builds).

## Contributing

- Follow `.github/copilot/instructions.md` for AI-agent and repository rules.
- Use English for code, comments and commits.


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy solution and project files for restore (paths are relative to build context root)
# use repository-root paths so Docker can find them when context is the repo root
COPY ProdSight.sln ./
COPY src/ProdSight.Api/ ./src/ProdSight.Api/
COPY src/ProdSight.Api.Shared/ ./src/ProdSight.Api.Shared/
COPY src/ProdSight.Api.Shared.DTOs/ ./src/ProdSight.Api.Shared.DTOs/
COPY src/ProdSight.Api.Modules/ ./src/ProdSight.Api.Modules/

WORKDIR /src/src/ProdSight.Api
RUN dotnet restore ProdSight.Api.csproj

# publish according to existing project rules (MSBuild target will run)
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# copy published output as-is (includes modules/ subfolders created by MSBuild target)
COPY --from=build /app/publish/ ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ProdSight.Api.dll"]

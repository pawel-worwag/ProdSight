ARG VERSION=unspecified
ARG GIT_SHA=unspecified

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy solution and project files for restore (paths are relative to build context root)
# use repository-root paths so Docker can find them when context is the repo root
COPY ProdSight.sln ./
COPY Directory.Build.props ./
COPY src/ProdSight.Api/ ./src/ProdSight.Api/
COPY src/ProdSight.Api.Shared/ ./src/ProdSight.Api.Shared/
COPY src/ProdSight.Api.Shared.DTOs/ ./src/ProdSight.Api.Shared.DTOs/
COPY src/ProdSight.Api.Modules/ ./src/ProdSight.Api.Modules/

WORKDIR /src/src/ProdSight.Api
RUN dotnet restore ProdSight.Api.csproj

# publish according to existing project rules (MSBuild target will run)
# pass Version from build arg so assembly version matches git tag/derived version
RUN dotnet publish -c Release -o /app/publish -p:Version=$VERSION

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
ARG VERSION=unspecified
ARG GIT_SHA=unspecified
WORKDIR /app

# copy published output as-is (includes modules/ subfolders created by MSBuild target)
COPY --from=build /app/publish/ ./

# metadata
LABEL org.opencontainers.image.version=$VERSION
LABEL org.opencontainers.image.revision=$GIT_SHA

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ProdSight.Api.dll"]

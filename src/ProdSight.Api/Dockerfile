# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy solution and project files for restore (adjust paths if necessary)
COPY ../../ProdSight.sln ./
COPY . ./src/ProdSight.Api/
COPY ../ProdSight.Api.Shared* ./src/
COPY ../ProdSight.Api.Modules ./src/ProdSight.Api.Modules/

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

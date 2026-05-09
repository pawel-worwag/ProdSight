#!/usr/bin/env pwsh

<#
.SYNOPSIS

.PARAMETER ModuleName
  Name od application module (part of path)

.PARAMETER AreaName
  Name of area (part of path)

.PARAMETER FeatureName
  Name of feature (type and endpoint name)
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $ModuleName,
    [Parameter(Mandatory = $true)] [string] $AreaName,
    [Parameter(Mandatory = $true)] [string] $FeatureName
)

function Build-SliceDtoFile([string]$module, [string]$area, [string]$feature)
{
    @"
using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.$module.$area.$feature;

public sealed record Request
{
    [JsonPropertyName("id")] public required Guid Id { get; init; } 
}
"@
}

function Build-SliceFile([string]$module, [string]$area, [string]$feature)
{
    @"
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProdSight.Api.Shared.Api;
using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Messaging;
using DTOs = ProdSight.Api.Shared.DTOs.$module.$area.$feature;

namespace ProdSight.Api.Modules.$module.Application.Features.$feature;

public static class $feature
{
    public sealed record Request(/* TODO */):IRequest</* TODO */>;
    
    public sealed class Handler(/* TODO */) : IRequestHandler<Request,/* TODO */>
    {
        public async Task</* ToDO */> HandleAsync(Request query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
    
    public sealed class Endpoint : IApiEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(/* TODO */,ExecuteAsync)
            .WithTags([/* TODO */])
                .WithSummary(/* TODO */)
                .Produces</* TODO */>()
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json");
        }
        
        private static async Task<IResult> ExecuteAsync(IRequestHandler<Request, /* TODO */> handler, CancellationToken ct)
        {
            var result = await handler.HandleAsync(new Request(), ct);
            return Results.Ok(result);
        }
    }
}
"@
}


$folder = "./src/ProdSight.Api.Modules/$ModuleName/Application/Features/$AreaName/"
$featureClassFile = "$folder$FeatureName.cs"


$dtoFolder = "./src/ProdSight.Api.Shared.DTOs/$ModuleName/$AreaName/$FeatureName/"
$dtoRequestFile = "${dtoFolder}Request.cs"

Write-Host $folder
Write-Host $featureClassFile
Write-Host $dtoFolder
Write-Host $dtoRequestFile

if (-not (Test-Path $folder)) {
    New-Item -Path $folder -ItemType Directory | Out-Null
}

if (-not (Test-Path $dtoFolder)) {
    New-Item -Path $dtoFolder -ItemType Directory | Out-Null
}

if (Test-Path $featureClassFile) {
    Write-Host "Class file exists"
    throw "file '$featureClassFile' Exists"
}

if (Test-Path $featureClassFile) {
    Write-Host "Class file exists"
    throw "file '$featureClassFile' Exists"
}


Set-Content -Path $featureClassFile -Value (Build-SliceFile $ModuleName $AreaName $FeatureName)
Set-Content -Path $dtoRequestFile -Value (Build-SliceDtoFile $ModuleName $AreaName $FeatureName)
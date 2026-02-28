using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ProdSight.Api.Shared.DTOs.Health;
using ProdSight.Frontend.Api;

namespace ProdSight.Frontend.Pages;

public partial class Health(IApiBroker api) : ComponentBase, IAsyncDisposable
{
    private HealthResponse? HealthStatus { get; set; }

    private CancellationTokenSource? _cts;
    private Task? _pollingTask;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _cts = new CancellationTokenSource();
        _pollingTask = StartPollingAsync(_cts.Token);
    }

    private async Task StartPollingAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var status = await api.GetHealthStatusAsync();
                    HealthStatus = status;
                    await InvokeAsync(StateHasChanged);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    // swallow individual fetch errors (optionally log)
                }

                await Task.Delay(_pollInterval, token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            if (_pollingTask != null)
            {
                try
                {
                    await _pollingTask;
                }
                catch (OperationCanceledException)
                {
                }
            }

            _cts.Dispose();
            _cts = null;
        }
    }
}
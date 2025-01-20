using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using Travelog.Core.Abstractions;
using Travelog.Core.Models;

public class BackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public BackgroundService(IBackgroundTaskQueue backgroundTaskQueue)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _backgroundTaskQueue.ProcessQueueAsync(stoppingToken);    
    }
}

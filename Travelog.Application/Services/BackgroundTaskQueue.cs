using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Travelog.Core.Abstractions;

namespace Travelog.Application.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public BackgroundTaskQueue(IServiceScopeFactory serviceScopeFactory)
        {
            _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
        }

        public async Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }

        public async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            await foreach (var workItem in _queue.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                        await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}

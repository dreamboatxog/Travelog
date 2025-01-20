namespace Travelog.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);
        Task ProcessQueueAsync(CancellationToken cancellationToken);
    }

}

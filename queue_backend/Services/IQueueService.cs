using Dotnet.Models;

namespace Dotnet.Services
{
    /// <summary>
    /// Abstraction for queue operations and queue state access.
    /// </summary>
    public interface IQueueService
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Enqueue a new payload into the queue.
        /// </summary>
        /// <param name="payload">The payload to enqueue.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Details about the enqueued item, including its position.</returns>
        Task<EnqueueResponse> EnqueueAsync(string payload, CancellationToken ct = default);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Dequeue the next item from the queue.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The dequeued item or null if the queue is empty.</returns>
        Task<QueueItem?> DequeueAsync(CancellationToken ct = default);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get the current status of the queue.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Status information including the current item count and whether the queue is empty.</returns>
        Task<QueueStatusResponse> GetStatusAsync(CancellationToken ct = default);
    }
}

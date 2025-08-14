namespace Dotnet.Models
{
    /// <summary>
    /// Represents an item stored in the queue.
    /// </summary>
    public class QueueItem
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Unique identifier for the queue item.
        /// </summary>
        public Guid Id { get; set; }

        // PUBLIC_INTERFACE
        /// <summary>
        /// The payload string.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        // PUBLIC_INTERFACE
        /// <summary>
        /// UTC timestamp when the item was enqueued.
        /// </summary>
        public DateTimeOffset EnqueuedAt { get; set; }
    }
}

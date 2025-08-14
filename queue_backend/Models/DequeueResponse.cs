namespace Dotnet.Models
{
    /// <summary>
    /// Response returned when an item is dequeued.
    /// </summary>
    public class DequeueResponse
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// The dequeued queue item.
        /// </summary>
        public QueueItem Item { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DequeueResponse"/>.
        /// </summary>
        public DequeueResponse(QueueItem item)
        {
            Item = item;
        }
    }
}

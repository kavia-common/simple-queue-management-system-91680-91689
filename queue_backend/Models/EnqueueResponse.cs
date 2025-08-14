namespace Dotnet.Models
{
    /// <summary>
    /// Response returned when an item is enqueued.
    /// </summary>
    public class EnqueueResponse
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// The enqueued item.
        /// </summary>
        public QueueItem Item { get; }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Approximate position in the queue after enqueue.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnqueueResponse"/>.
        /// </summary>
        public EnqueueResponse(QueueItem item, int position)
        {
            Item = item;
            Position = position;
        }
    }
}

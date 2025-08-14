namespace Dotnet.Models
{
    /// <summary>
    /// Represents current queue status information.
    /// </summary>
    public class QueueStatusResponse
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Current count of items in the queue.
        /// </summary>
        public int Count { get; }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Whether the queue is empty.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="QueueStatusResponse"/>.
        /// </summary>
        public QueueStatusResponse(int count, bool isEmpty)
        {
            Count = count;
            IsEmpty = isEmpty;
        }
    }
}

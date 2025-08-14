using System.ComponentModel.DataAnnotations;

namespace Dotnet.Models
{
    /// <summary>
    /// Request body for enqueue operation.
    /// </summary>
    public class EnqueueRequest
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Payload to enqueue.
        /// </summary>
        [Required]
        public string Payload { get; set; } = string.Empty;
    }
}

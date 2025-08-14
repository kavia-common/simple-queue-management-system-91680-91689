using System.Collections.Concurrent;
using System.Text.Json;
using Dotnet.Models;
using Microsoft.Extensions.Logging;

namespace Dotnet.Services
{
    /// <summary>
    /// In-memory queue service with JSON file persistence.
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly ConcurrentQueue<QueueItem> _queue = new();
        private readonly SemaphoreSlim _persistenceLock = new(1, 1);
        private readonly string _storageFilePath;
        private readonly ILogger<QueueService> _logger;
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueService"/>.
        /// Loads persisted queue state from disk if available.
        /// </summary>
        /// <param name="configuration">App configuration used to determine the storage path.</param>
        /// <param name="env">Hosting environment to resolve default content root path.</param>
        /// <param name="logger">Logger instance.</param>
        public QueueService(IConfiguration configuration, IWebHostEnvironment env, ILogger<QueueService> logger)
        {
            _logger = logger;
            // Resolve storage path from configuration or use default under content root.
            _storageFilePath = configuration["QueueStorage:FilePath"] ??
                               Path.Combine(env.ContentRootPath, "queue_state.json");

            try
            {
                LoadFromDisk();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load queue state from disk. Starting with an empty queue.");
            }
        }

        // PUBLIC_INTERFACE
        /// <inheritdoc />
        public async Task<EnqueueResponse> EnqueueAsync(string payload, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentException("Payload cannot be empty.", nameof(payload));
            }

            var item = new QueueItem
            {
                Id = Guid.NewGuid(),
                Payload = payload,
                EnqueuedAt = DateTimeOffset.UtcNow
            };

            _queue.Enqueue(item);

            // Position after enqueue is an approximation based on current count
            var position = _queue.Count;

            await PersistAsync(ct).ConfigureAwait(false);

            return new EnqueueResponse(item, position);
        }

        // PUBLIC_INTERFACE
        /// <inheritdoc />
        public async Task<QueueItem?> DequeueAsync(CancellationToken ct = default)
        {
            if (_queue.TryDequeue(out var item))
            {
                await PersistAsync(ct).ConfigureAwait(false);
                return item;
            }

            return null;
        }

        // PUBLIC_INTERFACE
        /// <inheritdoc />
        public Task<QueueStatusResponse> GetStatusAsync(CancellationToken ct = default)
        {
            var count = _queue.Count;
            var status = new QueueStatusResponse(count, count == 0);
            return Task.FromResult(status);
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_storageFilePath))
            {
                _logger.LogInformation("Queue storage file not found at {Path}. Starting with empty queue.", _storageFilePath);
                return;
            }

            var json = File.ReadAllText(_storageFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Queue storage file at {Path} is empty. Starting with empty queue.", _storageFilePath);
                return;
            }

            var items = JsonSerializer.Deserialize<List<QueueItem>>(json, SerializerOptions) ?? new List<QueueItem>();

            // Rehydrate the queue maintaining order
            foreach (var it in items.OrderBy(i => i.EnqueuedAt))
            {
                _queue.Enqueue(it);
            }

            _logger.LogInformation("Loaded {Count} queue items from {Path}.", items.Count, _storageFilePath);
        }

        private async Task PersistAsync(CancellationToken ct)
        {
            // Snapshot current queue
            var items = _queue.ToArray().OrderBy(i => i.EnqueuedAt).ToList();

            Directory.CreateDirectory(Path.GetDirectoryName(_storageFilePath) ?? ".");

            await _persistenceLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                using var stream = new FileStream(_storageFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(stream, items, SerializerOptions, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist queue state to {Path}.", _storageFilePath);
            }
            finally
            {
                _persistenceLock.Release();
            }
        }
    }
}

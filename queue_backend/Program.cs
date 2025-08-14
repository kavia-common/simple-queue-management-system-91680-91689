using Dotnet.Models;
using Dotnet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    // App-level OpenAPI metadata
    settings.PostProcess = document =>
    {
        document.Info.Title = "Queue Backend API";
        document.Info.Version = "v1";
        document.Info.Description = "A simple queue management system with REST endpoints to enqueue, dequeue, and check queue status. No authentication required.";
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Queue service (in-memory with file persistence)
builder.Services.AddSingleton<IQueueService, QueueService>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

 // Health check endpoint
app.MapGet("/", () => new { message = "Healthy" })
   .WithName("HealthCheck")
   .WithTags("System");

// Enqueue endpoint
app.MapPost("/api/queue/enqueue", async (EnqueueRequest request, IQueueService queueService, CancellationToken ct) =>
{
    if (request is null || string.IsNullOrWhiteSpace(request.Payload))
    {
        return Results.BadRequest(new { error = "Payload is required." });
    }

    var result = await queueService.EnqueueAsync(request.Payload, ct);
    return Results.Created($"/api/queue/items/{result.Item.Id}", result);
})
.WithName("Enqueue")
.WithTags("Queue");

// Dequeue endpoint
app.MapPost("/api/queue/dequeue", async (IQueueService queueService, CancellationToken ct) =>
{
    var item = await queueService.DequeueAsync(ct);
    if (item is null)
    {
        return Results.NoContent(); // 204 if queue is empty
    }
    return Results.Ok(new DequeueResponse(item));
})
.WithName("Dequeue")
.WithTags("Queue");

// Status endpoint
app.MapGet("/api/queue/status", async (IQueueService queueService, CancellationToken ct) =>
{
    var status = await queueService.GetStatusAsync(ct);
    return Results.Ok(status);
})
.WithName("QueueStatus")
.WithTags("Queue");

app.Run();

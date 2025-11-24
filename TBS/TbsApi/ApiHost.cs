using System.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Read Redis URL from environment
var cfg = builder.Configuration;
string? redisUrl = cfg["REDIS_URL"];

if (string.IsNullOrWhiteSpace(redisUrl))
{
    throw new InvalidOperationException("Missing REDIS_URL environment variable.");
}

// Parse Redis URL (format: redis://host:port or rediss://host:port)
var redisUri = new Uri(redisUrl);
var redisConfig = new ConfigurationOptions
{
    EndPoints = { { redisUri.Host, redisUri.Port } },
    Ssl = redisUri.Scheme == "rediss",
    AbortOnConnectFail = false,
    ConnectTimeout = 10000,
    SyncTimeout = 5000
};

// Add password if present
if (!string.IsNullOrEmpty(redisUri.UserInfo))
{
    var parts = redisUri.UserInfo.Split(':');
    if (parts.Length > 1)
    {
        redisConfig.Password = parts[1];
    }
}

IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConfig);
IDatabase db = redis.GetDatabase();

builder.Services.AddSingleton(redis);
builder.Services.AddSingleton(db);

static string Canonical(string name) => (name ?? string.Empty).Trim().ToLowerInvariant();
static string PlayerKey(string canonical) => $"player:{canonical}";
static string DeadPlayerKey(string canonical) => $"dead:{canonical}";

var app = builder.Build();

// Health check
app.MapGet("/", (HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";
    return Results.Text("pong", "text/plain");
});

app.MapGet("/ping", (HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";
    return Results.Text("pong", "text/plain");
});

app.MapGet("/health", (HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";
    return Results.Text("pong", "text/plain");
});

app.MapGet("/debug/info", (HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";
    return Results.Json(new
    {
        storage = "redis",
        connected = redis.IsConnected,
        endpoint = redisUrl?.Split('@').LastOrDefault()?.Split('?').FirstOrDefault() ?? "unknown"
    });
});

// Save/update player
app.MapPut("/players/{name}", async (string name, HttpRequest request, IDatabase db, HttpResponse resp) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: false);
    string json = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(json))
        return Results.BadRequest("Request body must contain player JSON.");

    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    // Write to Redis
    await db.StringSetAsync(key, json);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Key"] = key;
    return Results.NoContent();
});

// Get player
app.MapGet("/players/{name}", async (string name, IDatabase db, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    var json = await db.StringGetAsync(key);

    if (!json.HasValue)
    {
        resp.Headers["Cache-Control"] = "no-store";
        resp.Headers["X-TBS-Storage"] = "redis";
        resp.Headers["X-TBS-Key"] = key;
        return Results.NotFound();
    }

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Key"] = key;
    return Results.Text(json!, "application/json", Encoding.UTF8);
});

// Delete player
app.MapDelete("/players/{name}", async (string name, IDatabase db, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    await db.KeyDeleteAsync(key);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Deleted-Canonical"] = canonical;
    return Results.NoContent();
});

// Save dead player
app.MapPut("/dead-players/{name}", async (string name, HttpRequest request, IDatabase db, HttpResponse resp) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: false);
    string json = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(json))
        return Results.BadRequest("Request body must contain player JSON.");

    var canonical = Canonical(name);
    string key = DeadPlayerKey(canonical);

    await db.StringSetAsync(key, json);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Key"] = key;
    return Results.NoContent();
});

// Get dead player
app.MapGet("/dead-players/{name}", async (string name, IDatabase db, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = DeadPlayerKey(canonical);

    var json = await db.StringGetAsync(key);

    if (!json.HasValue)
    {
        resp.Headers["Cache-Control"] = "no-store";
        resp.Headers["X-TBS-Storage"] = "redis";
        resp.Headers["X-TBS-Key"] = key;
        return Results.NotFound();
    }

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Key"] = key;
    return Results.Text(json!, "application/json", Encoding.UTF8);
});

// Delete dead player
app.MapDelete("/dead-players/{name}", async (string name, IDatabase db, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = DeadPlayerKey(canonical);

    await db.KeyDeleteAsync(key);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Storage"] = "redis";
    resp.Headers["X-TBS-Deleted-Canonical"] = canonical;
    return Results.NoContent();
});

// List all dead players
app.MapGet("/dead-players", async (IDatabase db, IConnectionMultiplexer redis, HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";

    try
    {
        var playerNames = new List<string>();
        var server = redis.GetServer(redis.GetEndPoints().First());

        // Scan for keys matching pattern "dead:*"
        await foreach (var key in server.KeysAsync(pattern: "dead:*"))
        {
            var keyStr = key.ToString();
            if (keyStr.StartsWith("dead:"))
            {
                playerNames.Add(keyStr.Substring(5)); // Remove "dead:" prefix
            }
        }

        return Results.Json(playerNames);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error listing dead players: {ex.Message}");
    }
});

app.Run();
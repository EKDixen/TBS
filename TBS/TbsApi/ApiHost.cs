using System.Net;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

var builder = WebApplication.CreateBuilder(args);

// Read required configuration from environment variables (available via builder.Configuration)
var cfg = builder.Configuration;
string? endpoint = cfg["B2_S3_ENDPOINT"];       // e.g. https://s3.us-east-005.backblazeb2.com
string? region  = cfg["B2_REGION"];             // e.g. us-east-005
string? bucket  = cfg["B2_BUCKET"];             // e.g. TBS-playerdatabase
string? keyId   = cfg["B2_KEY_ID"];             // Application Key ID
string? appKey  = cfg["B2_APP_KEY"];            // Application Key Secret

if (string.IsNullOrWhiteSpace(endpoint) ||
    string.IsNullOrWhiteSpace(region) ||
    string.IsNullOrWhiteSpace(bucket) ||
    string.IsNullOrWhiteSpace(keyId) ||
    string.IsNullOrWhiteSpace(appKey))
{
    throw new InvalidOperationException("Missing required B2 configuration. Ensure B2_S3_ENDPOINT, B2_REGION, B2_BUCKET, B2_KEY_ID, and B2_APP_KEY are set.");
}

var s3Config = new AmazonS3Config
{
    ServiceURL = endpoint,
    AuthenticationRegion = region,
    ForcePathStyle = true,
};

IAmazonS3 s3Client = new AmazonS3Client(keyId, appKey, s3Config);

builder.Services.AddSingleton(s3Client);
builder.Services.AddSingleton(new BucketOptions(bucket!));

static string Canonical(string name) => (name ?? string.Empty).Trim().ToLowerInvariant();
static string PlayerKey(string canonical) => $"players/{canonical}.json";

static async Task DeleteKeyWithVersionsAsync(IAmazonS3 s3, string bucket, string key, CancellationToken ct)
{
    try
    {
        var verReq = new ListVersionsRequest { BucketName = bucket, Prefix = key };
        ListVersionsResponse verResp;
        do
        {
            verResp = await s3.ListVersionsAsync(verReq, ct);
            foreach (var v in verResp.Versions)
            {
                if (!string.Equals(v.Key, key, StringComparison.Ordinal)) continue;
                try
                {
                    var delReq = new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = key,
                        VersionId = v.VersionId
                    };
                    await s3.DeleteObjectAsync(delReq, ct);
                }
                catch { }
            }
            foreach (var d in verResp.DeleteMarkers)
            {
                if (!string.Equals(d.Key, key, StringComparison.Ordinal)) continue;
                try
                {
                    var delReq = new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = key,
                        VersionId = d.VersionId
                    };
                    await s3.DeleteObjectAsync(delReq, ct);
                }
                catch { }
            }
            verReq.KeyMarker = verResp.NextKeyMarker;
            verReq.VersionIdMarker = verResp.NextVersionIdMarker;
        } while (verResp.IsTruncated);
    }
    catch { }

    try { await s3.DeleteObjectAsync(bucket, key, ct); } catch { }
}

var app = builder.Build();

// Health check
app.MapGet("/", (HttpResponse resp) => { resp.Headers["Cache-Control"] = "no-store"; return Results.Text("testing", "text/plain"); });
app.MapGet("/ping", (HttpResponse resp) => { resp.Headers["Cache-Control"] = "no-store"; return Results.Text("pong", "text/plain"); });
app.MapGet("/health", (HttpResponse resp) => { resp.Headers["Cache-Control"] = "no-store"; return Results.Text("i am health (trust)", "text/plain"); });

app.MapGet("/debug/info", (BucketOptions bucketOpt, HttpResponse resp) =>
{
    resp.Headers["Cache-Control"] = "no-store";
    return Results.Json(new
    {
        storage = "backblaze-b2-s3",
        bucket = bucketOpt.Name,
        endpoint,
        region
    });
});

app.MapPut("/players/{name}", async (string name, HttpRequest request, IAmazonS3 s3, BucketOptions bucketOpt, CancellationToken ct, HttpResponse resp) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: false);
    string json = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(json))
        return Results.BadRequest("Request body must contain player JSON.");

    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    var putReq = new PutObjectRequest
    {
        BucketName = bucketOpt.Name,
        Key = key,
        ContentType = "application/json",
        // Omit SSE AES256 to avoid Backblaze rejection
        InputStream = new MemoryStream(Encoding.UTF8.GetBytes(json))
    };

    await s3.PutObjectAsync(putReq, ct);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Bucket"] = bucketOpt.Name;
    resp.Headers["X-TBS-Key"] = key;
    return Results.NoContent();
});

app.MapGet("/players/{name}", async (string name, IAmazonS3 s3, BucketOptions bucketOpt, CancellationToken ct, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    try
    {
        using var s3resp = await s3.GetObjectAsync(bucketOpt.Name, key, ct);
        using var reader = new StreamReader(s3resp.ResponseStream, Encoding.UTF8);
        string json = await reader.ReadToEndAsync();
        resp.Headers["Cache-Control"] = "no-store";
        resp.Headers["X-TBS-Bucket"] = bucketOpt.Name;
        resp.Headers["X-TBS-Key"] = key;
        return Results.Text(json, "application/json", Encoding.UTF8);
    }
    catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.ErrorCode == "NoSuchKey")
    {
        resp.Headers["Cache-Control"] = "no-store";
        resp.Headers["X-TBS-Bucket"] = bucketOpt.Name;
        resp.Headers["X-TBS-Key"] = key;
        return Results.NotFound();
    }
});

app.MapDelete("/players/{name}", async (string name, IAmazonS3 s3, BucketOptions bucketOpt, CancellationToken ct, HttpResponse resp) =>
{
    var canonical = Canonical(name);
    string key = PlayerKey(canonical);

    await DeleteKeyWithVersionsAsync(s3, bucketOpt.Name, key, ct);

    resp.Headers["Cache-Control"] = "no-store";
    resp.Headers["X-TBS-Bucket"] = bucketOpt.Name;
    resp.Headers["X-TBS-Deleted-Canonical"] = canonical;
    return Results.NoContent();
});

app.Run();

internal record BucketOptions(string Name);
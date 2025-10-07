using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

var builder = WebApplication.CreateBuilder(args);

// Read required configuration from environment variables (available via builder.Configuration)
string? endpoint = builder.Configuration["B2_S3_ENDPOINT"];       // e.g. https://s3.us-east-005.backblazeb2.com
string? region = builder.Configuration["B2_REGION"];              // e.g. us-east-005
string? bucket = builder.Configuration["B2_BUCKET"];              // e.g. TBS-playerdatabase
string? keyId = builder.Configuration["B2_KEY_ID"];              // Application Key ID
string? appKey = builder.Configuration["B2_APP_KEY"];            // Application Key Secret

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
builder.Services.AddSingleton(new BucketOptions(bucket));

var app = builder.Build();

// Health check
app.MapGet("/ping", () => Results.Text("pong", "text/plain"));

// PUT /players/{name} -> uploads JSON to players/{name}.json
app.MapPut("/players/{name}", async (string name, HttpRequest request, IAmazonS3 s3, BucketOptions bucketOpt, CancellationToken ct) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: false);
    string json = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(json))
        return Results.BadRequest("Request body must contain player JSON.");

    string key = $"players/{name}.json";

    var putReq = new PutObjectRequest
    {
        BucketName = bucketOpt.Name,
        Key = key,
        ContentType = "application/json",
        ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
        InputStream = new MemoryStream(Encoding.UTF8.GetBytes(json))
    };

    await s3.PutObjectAsync(putReq, ct);
    return Results.NoContent();
});

// GET /players/{name} -> downloads and returns JSON
app.MapGet("/players/{name}", async (string name, IAmazonS3 s3, BucketOptions bucketOpt, CancellationToken ct) =>
{
    string key = $"players/{name}.json";
    try
    {
        using var resp = await s3.GetObjectAsync(bucketOpt.Name, key, ct);
        using var reader = new StreamReader(resp.ResponseStream, Encoding.UTF8);
        string json = await reader.ReadToEndAsync();
        return Results.Text(json, "application/json", Encoding.UTF8);
    }
    catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.ErrorCode == "NoSuchKey")
    {
        return Results.NotFound();
    }
});

app.Run();

internal record BucketOptions(string Name);

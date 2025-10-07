using System.Net;
using System.Net.Http;
using System.Text;

public class PlayerDatabase
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    // baseUrl example: http://localhost:5076
    public PlayerDatabase(string? baseUrl = null)
    {
        // Build base URL from argument or environment, sanitize and validate
        var raw = string.IsNullOrWhiteSpace(baseUrl)
            ? Environment.GetEnvironmentVariable("TBS_API_BASEURL")
            : baseUrl;

        var url = (raw ?? "https://tbs-wlt8.onrender.com").Trim();
        url = url.Trim('\"', '\'').Trim(); // remove accidental quotes
        url = url.TrimEnd('/');

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // prefer https for hosted API URLs
            url = "https://" + url;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed) ||
            (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException($"Invalid API base URL: '{url}'. Set TBS_API_BASEURL to a valid http(s) URL.");
        }

        _baseUrl = parsed.ToString().TrimEnd('/');
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    // Synchronous wrapper around HttpClient to avoid changing existing callers
    public void SavePlayer(Player p)
    {
        string json = Serializer.ToJson(p);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";

        using var resp = _http.PutAsync(url, content).GetAwaiter().GetResult();
        resp.EnsureSuccessStatusCode();
    }

    public Player? LoadPlayer(string username, string password)
    {
        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(username)}";
        using var resp = _http.GetAsync(url).GetAwaiter().GetResult();

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var player = Serializer.FromJson<Player>(json);
        if (player.password == password)
            return player;
        return null;
    }

    public bool PlayerExists(string username)
    {
        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(username)}";
        using var resp = _http.GetAsync(url).GetAwaiter().GetResult();
        return resp.StatusCode == HttpStatusCode.OK;
    }
}

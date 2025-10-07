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
        var url = string.IsNullOrWhiteSpace(baseUrl)
            ? (Environment.GetEnvironmentVariable("TBS_API_BASEURL") ?? "https://tbs-jgfg.onrender.com")
            : baseUrl;

        url = url.TrimEnd('/');
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            url = "http://" + url;

        _baseUrl = url;
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

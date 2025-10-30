using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class PlayerDatabase
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public PlayerDatabase(string? baseUrl = null)
    {
        // Build base URL from argument or environment, sanitize and validate
        var raw = string.IsNullOrWhiteSpace(baseUrl)
            ? Environment.GetEnvironmentVariable("TBS_API_BASEURL")
            : baseUrl;

        var url = (raw ?? "https://tbs-wlt8.onrender.com").Trim();
        url = url.Trim('"', '\'').Trim();
        url = url.TrimEnd('/');

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
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

    private bool IsServerAvailable()
    {
        try
        {
            using var hc = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var healthUrl = $"{_baseUrl}/health";
            using var resp = hc.GetAsync(healthUrl).GetAwaiter().GetResult();
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private void WaitForServerReady(TimeSpan? maxWait = null)
    {
        var timeout = maxWait ?? TimeSpan.FromSeconds(90);
        var start = DateTime.UtcNow;
        bool printed = false;
        while (!IsServerAvailable())
        {
            if (!printed)
            {
                Console.Write("Waiting for server");
                printed = true;
            }
            Console.Write(".");
            Thread.Sleep(1000);
            if (DateTime.UtcNow - start > timeout)
                break;
        }
        if (printed) Console.WriteLine();
    }

    private void PruneLegacyLocalSaves(string username, int keep = 1)
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var dirs = new[]
            {
                Path.Combine(baseDir, "Saves", username),
                Path.Combine(baseDir, "Saves", username.ToLowerInvariant()),
                Path.Combine(baseDir, "players", username),
            };

            foreach (var dir in dirs)
            {
                if (!Directory.Exists(dir)) continue;

                var files = Directory.EnumerateFiles(dir, "*.*")
                    .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".save", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".sav", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => File.GetLastWriteTimeUtc(f))
                    .ToList();

                for (int i = keep; i < files.Count; i++)
                {
                    try { File.Delete(files[i]); } catch {}
                }

                foreach (var tmp in Directory.EnumerateFiles(dir, "*.tmp"))
                {
                    try { File.Delete(tmp); } catch {}
                }
            }
        }
        catch {}
    }

    public void SavePlayer(Player p)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        string json = Serializer.ToJson(p);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.PutAsync(url, content).GetAwaiter().GetResult();
                resp.EnsureSuccessStatusCode();
                PruneLegacyLocalSaves(p.name, keep: 1);
                return;
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Server unavailable, retrying save...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using var finalResp = _http.PutAsync(url, content).GetAwaiter().GetResult();
        finalResp.EnsureSuccessStatusCode();
        PruneLegacyLocalSaves(p.name, keep: 1);
    }

    public Player? LoadPlayer(string username, string password)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(username)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
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
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Waiting on server...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using (var resp2 = _http.GetAsync(url).GetAwaiter().GetResult())
        {
            if (resp2.StatusCode == HttpStatusCode.NotFound)
                return null;

            resp2.EnsureSuccessStatusCode();
            string json = resp2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var player = Serializer.FromJson<Player>(json);
            if (player.password == password)
                return player;
            return null;
        }
    }

    public bool PlayerExists(string username)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        var url = $"{_baseUrl}/players/{Uri.EscapeDataString(username)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.GetAsync(url).GetAwaiter().GetResult();
                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Waiting on server...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using var final = _http.GetAsync(url).GetAwaiter().GetResult();
        return final.StatusCode == HttpStatusCode.OK;
    }

    public void MarkPlayerAsDead(Player p)
    {
        try
        {
            WaitForServerReady(TimeSpan.FromSeconds(90));

            p.isDead = true;
            p.HP = p.maxHP;
<<<<<<< Updated upstream
            
            Console.WriteLine($"[DEBUG] Marking {p.name} as dead...");
            Console.WriteLine($"[DEBUG] Location: {p.currentLocation}");
=======
>>>>>>> Stashed changes
            
            string json = Serializer.ToJson(p);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var deadUrl = $"{_baseUrl}/dead-players/{Uri.EscapeDataString(p.name)}";
            
            Console.WriteLine($"[DEBUG] Saving to: {deadUrl}");

            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using var resp = _http.PutAsync(deadUrl, content).GetAwaiter().GetResult();
                    
                    Console.WriteLine($"[DEBUG] PUT response: {resp.StatusCode}");
                    
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Dead players endpoint not available, marking player as dead in active database...");
                        var activeUrl = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";
                        var activeContent = new StringContent(json, Encoding.UTF8, "application/json");
                        using var activeResp = _http.PutAsync(activeUrl, activeContent).GetAwaiter().GetResult();
                        activeResp.EnsureSuccessStatusCode();
                        DeadPlayerCache.AddDeadPlayer(p.name, p.currentLocation);
                        return;
                    }
                    
                    resp.EnsureSuccessStatusCode();
                    
                    var deleteUrl = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";
                    Console.WriteLine($"[DEBUG] Deleting from active players...");
                    using var deleteResp = _http.DeleteAsync(deleteUrl).GetAwaiter().GetResult();
                    deleteResp.EnsureSuccessStatusCode();
                    
<<<<<<< Updated upstream
                    Console.WriteLine("[DEBUG] SUCCESS: Player moved to dead players database.");
=======
                    Console.WriteLine("Player moved to dead players database.");
                    DeadPlayerCache.AddDeadPlayer(p.name, p.currentLocation);
>>>>>>> Stashed changes
                    return;
                }
                catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
                {
                    Console.WriteLine("Server unavailable, retrying...");
                    WaitForServerReady(TimeSpan.FromSeconds(90));
                }
            }

            using (var resp = _http.PutAsync(deadUrl, content).GetAwaiter().GetResult())
            {
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    var activeUrl = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";
                    var activeContent = new StringContent(json, Encoding.UTF8, "application/json");
                    using var activeResp = _http.PutAsync(activeUrl, activeContent).GetAwaiter().GetResult();
                    activeResp.EnsureSuccessStatusCode();
                    DeadPlayerCache.AddDeadPlayer(p.name, p.currentLocation);
                    return;
                }
                resp.EnsureSuccessStatusCode();
            }
            
            var finalActiveUrl = $"{_baseUrl}/players/{Uri.EscapeDataString(p.name)}";
            using (var deleteResp = _http.DeleteAsync(finalActiveUrl).GetAwaiter().GetResult())
            {
                deleteResp.EnsureSuccessStatusCode();
            }
            
            DeadPlayerCache.AddDeadPlayer(p.name, p.currentLocation);
        }
        catch (Exception ex)
        {
<<<<<<< Updated upstream
            Console.WriteLine($"[DEBUG] ERROR marking player as dead: {ex.Message}");
            Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
=======
            Console.WriteLine($"Error marking player as dead: {ex.Message}");
            Console.WriteLine("Player death will be recorded locally.");
            DeadPlayerCache.AddDeadPlayer(p.name, p.currentLocation);
>>>>>>> Stashed changes
        }
    }

    public Player? LoadDeadPlayer(string username)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        var url = $"{_baseUrl}/dead-players/{Uri.EscapeDataString(username)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.GetAsync(url).GetAwaiter().GetResult();

                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return null;

                resp.EnsureSuccessStatusCode();
                string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return Serializer.FromJson<Player>(json);
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Waiting on server...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using (var resp2 = _http.GetAsync(url).GetAwaiter().GetResult())
        {
            if (resp2.StatusCode == HttpStatusCode.NotFound)
                return null;

            resp2.EnsureSuccessStatusCode();
            string json = resp2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return Serializer.FromJson<Player>(json);
        }
    }

<<<<<<< Updated upstream
    public List<string> GetAllDeadPlayerNames()
=======
    public List<Player> GetDeadPlayersInLocation(string locationName)
>>>>>>> Stashed changes
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        var url = $"{_baseUrl}/dead-players";
<<<<<<< Updated upstream
        
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.GetAsync(url).GetAwaiter().GetResult();
                
                if (!resp.IsSuccessStatusCode)
                {
                    return new List<string>();
                }
                
                string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var names = Serializer.FromJson<List<string>>(json);
                return names ?? new List<string>();
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        try
        {
            using var resp2 = _http.GetAsync(url).GetAwaiter().GetResult();
            
            if (!resp2.IsSuccessStatusCode)
            {
                return new List<string>();
            }
            
            string json2 = resp2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var names2 = Serializer.FromJson<List<string>>(json2);
            return names2 ?? new List<string>();
        }
        catch (Exception ex)
        {
            return new List<string>();
=======
        List<Player> deadPlayersInLocation = new List<Player>();

        try
        {
            Console.WriteLine("Warning: Listing dead players requires API enhancement. Returning empty list.");
            return deadPlayersInLocation;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching dead players: {ex.Message}");
            return deadPlayersInLocation;
>>>>>>> Stashed changes
        }
    }

    public void DeleteDeadPlayer(string username)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        var url = $"{_baseUrl}/dead-players/{Uri.EscapeDataString(username)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.DeleteAsync(url).GetAwaiter().GetResult();
                resp.EnsureSuccessStatusCode();
                Console.WriteLine($"Dead player {username} has been permanently removed.");
<<<<<<< Updated upstream
=======
                DeadPlayerCache.RemoveDeadPlayer(username);
>>>>>>> Stashed changes
                return;
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Server unavailable, retrying delete...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using var finalResp = _http.DeleteAsync(url).GetAwaiter().GetResult();
        finalResp.EnsureSuccessStatusCode();
<<<<<<< Updated upstream
=======
        DeadPlayerCache.RemoveDeadPlayer(username);
>>>>>>> Stashed changes
    }

    public void UpdateDeadPlayer(Player p)
    {
        WaitForServerReady(TimeSpan.FromSeconds(90));

        string json = Serializer.ToJson(p);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/dead-players/{Uri.EscapeDataString(p.name)}";

        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                using var resp = _http.PutAsync(url, content).GetAwaiter().GetResult();
                resp.EnsureSuccessStatusCode();
                return;
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is TimeoutException || ex is HttpRequestException || ex is IOException)
            {
                Console.WriteLine("Server unavailable, retrying update...");
                WaitForServerReady(TimeSpan.FromSeconds(90));
            }
        }

        using var finalResp = _http.PutAsync(url, content).GetAwaiter().GetResult();
        finalResp.EnsureSuccessStatusCode();
    }
}

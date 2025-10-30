using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DeadPlayerCache
{
    private static string cacheFilePath = Path.Combine(AppContext.BaseDirectory, "dead_players_cache.json");
    private static List<DeadPlayerEntry> cache = new List<DeadPlayerEntry>();

    public class DeadPlayerEntry
    {
        public string username { get; set; }
        public string location { get; set; }
    }

    static DeadPlayerCache()
    {
        LoadCache();
    }

    private static void LoadCache()
    {
        try
        {
            if (File.Exists(cacheFilePath))
            {
                string json = File.ReadAllText(cacheFilePath);
                cache = Serializer.FromJson<List<DeadPlayerEntry>>(json) ?? new List<DeadPlayerEntry>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dead player cache: {ex.Message}");
            cache = new List<DeadPlayerEntry>();
        }
    }

    private static void SaveCache()
    {
        try
        {
            string json = Serializer.ToJson(cache);
            File.WriteAllText(cacheFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving dead player cache: {ex.Message}");
        }
    }

    public static void AddDeadPlayer(string username, string location)
    {
        cache.RemoveAll(e => e.username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        cache.Add(new DeadPlayerEntry
        {
            username = username,
            location = location
        });
        
        SaveCache();
    }

    public static void RemoveDeadPlayer(string username)
    {
        cache.RemoveAll(e => e.username.Equals(username, StringComparison.OrdinalIgnoreCase));
        SaveCache();
    }

    public static List<string> GetDeadPlayersInLocation(string location)
    {
        return cache
            .Where(e => e.location.Equals(location, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.username)
            .ToList();
    }

    public static List<DeadPlayerEntry> GetAllDeadPlayers()
    {
        return new List<DeadPlayerEntry>(cache);
    }
}

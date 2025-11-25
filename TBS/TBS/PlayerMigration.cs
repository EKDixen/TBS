using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

/// <summary>
/// Migration script to add statTracker to existing players
/// Since the API doesn't support listing all players, you'll need to enter player names manually
/// </summary>
public class PlayerMigration
{
    private readonly PlayerDatabase db;
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public PlayerMigration(PlayerDatabase database, string baseUrl)
    {
        db = database;
        _baseUrl = baseUrl.TrimEnd('/');
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    /// <summary>
    /// Migrate multiple players by entering their names
    /// </summary>
    public void MigrateAllPlayers()
    {
        Console.WriteLine("=== PLAYER MIGRATION ===");
        Console.WriteLine("Note: Your API doesn't support listing all players.");
        Console.WriteLine("Enter player names one at a time to migrate them.\n");
        Console.WriteLine("Enter player names (one per line, empty line to finish):");

        var playerNames = new List<string>();
        while (true)
        {
            Console.Write("> ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                break;
            playerNames.Add(name.Trim());
        }

        if (playerNames.Count == 0)
        {
            Console.WriteLine("\nNo player names entered.");
            return;
        }

        Console.WriteLine($"\nMigrating {playerNames.Count} players...\n");
        
        int successCount = 0;
        int skipCount = 0;
        int errorCount = 0;

        foreach (var playerName in playerNames)
        {
            try
            {
                Console.Write($"Migrating {playerName}... ");
                
                var playerJson = GetPlayerJson(playerName);
                
                if (playerJson == null)
                {
                    Console.WriteLine("ERROR: Player not found");
                    errorCount++;
                    continue;
                }

                if (playerJson.Contains("\"statTracker\""))
                {
                    Console.WriteLine("SKIPPED (already has statTracker)");
                    skipCount++;
                    continue;
                }

                var player = Serializer.FromJson<Player>(playerJson);
                
                if (player == null)
                {
                    Console.WriteLine("ERROR: Could not deserialize player");
                    errorCount++;
                    continue;
                }

                if (player.statTracker == null)
                {
                    player.statTracker = new Dictionary<string, int>();
                }

                db.SavePlayer(player).Wait();
                
                Console.WriteLine("SUCCESS ✓");
                successCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                errorCount++;
            }
        }

        Console.WriteLine($"\n=== MIGRATION COMPLETE ===");
        Console.WriteLine($"Successfully migrated: {successCount}");
        Console.WriteLine($"Skipped (already migrated): {skipCount}");
        Console.WriteLine($"Errors: {errorCount}");
        Console.WriteLine($"Total: {playerNames.Count}");
    }

    /// <summary>
    /// Migrate a single player by name
    /// </summary>
    public bool MigrateSinglePlayer(string playerName)
    {
        try
        {
            Console.WriteLine($"Migrating player: {playerName}");
            
            var playerJson = GetPlayerJson(playerName);
            
            if (playerJson == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            if (playerJson.Contains("\"statTracker\""))
            {
                Console.WriteLine("Player already has statTracker field.");
                return true;
            }

            var player = Serializer.FromJson<Player>(playerJson);
            
            if (player == null)
            {
                Console.WriteLine("Could not deserialize player.");
                return false;
            }

            if (player.statTracker == null)
            {
                player.statTracker = new Dictionary<string, int>();
            }

            db.SavePlayer(player).Wait();
            
            Console.WriteLine("Migration successful!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get raw JSON for a player (to check if field exists)
    /// </summary>
    private string? GetPlayerJson(string playerName)
    {
        try
        {
            var url = $"{_baseUrl}/players/{Uri.EscapeDataString(playerName)}";
            using var resp = _http.GetAsync(url).GetAwaiter().GetResult();
            
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }
            
            return resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Dry run - shows what would be migrated without actually doing it
    /// </summary>
    public void DryRun()
    {
        Console.WriteLine("=== MIGRATION DRY RUN ===");
        Console.WriteLine("Note: Your API doesn't support listing all players.");
        Console.WriteLine("Enter player names one at a time to check their migration status.\n");
        Console.WriteLine("Enter player names (one per line, empty line to finish):");

        var playerNames = new List<string>();
        while (true)
        {
            Console.Write("> ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                break;
            playerNames.Add(name.Trim());
        }

        if (playerNames.Count == 0)
        {
            Console.WriteLine("\nNo player names entered.");
            return;
        }

        Console.WriteLine($"\nChecking {playerNames.Count} players:\n");
        
        int needsMigration = 0;
        int alreadyMigrated = 0;
        int notFound = 0;

        foreach (var playerName in playerNames)
        {
            try
            {
                var playerJson = GetPlayerJson(playerName);
                
                if (playerJson == null)
                {
                    Console.WriteLine($"  {playerName} - NOT FOUND");
                    notFound++;
                    continue;
                }

                if (playerJson.Contains("\"statTracker\""))
                {
                    Console.WriteLine($"  {playerName} - Already has statTracker ✓");
                    alreadyMigrated++;
                }
                else
                {
                    Console.WriteLine($"  {playerName} - NEEDS MIGRATION");
                    needsMigration++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {playerName} - ERROR: {ex.Message}");
            }
        }

        Console.WriteLine($"\n=== DRY RUN SUMMARY ===");
        Console.WriteLine($"Needs migration: {needsMigration}");
        Console.WriteLine($"Already migrated: {alreadyMigrated}");
        Console.WriteLine($"Not found: {notFound}");
        Console.WriteLine($"Total checked: {playerNames.Count}");
    }
}

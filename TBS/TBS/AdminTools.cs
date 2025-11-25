using System;

/// <summary>
/// Admin tool for database management and migrations
/// Call AdminTools.ShowAdminMenu() from your main Program.cs
/// </summary>
public static class AdminTools
{
    public static void ShowAdminMenu()
    {
        var db = new PlayerDatabase();
        var baseUrl = Environment.GetEnvironmentVariable("TBS_API_BASEURL") ?? "https://tbs-wlt8.onrender.com";
        var migration = new PlayerMigration(db, baseUrl);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║     TBS ADMIN TOOLS & MIGRATION        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("MIGRATION TOOLS:");
            Console.WriteLine("  1. Dry Run (preview migration)");
            Console.WriteLine("  2. Migrate All Players");
            Console.WriteLine("  3. Migrate Single Player");
            Console.WriteLine();
            Console.WriteLine("DATABASE CLEANUP:");
            Console.WriteLine("  4. Delete Single Player");
            Console.WriteLine("  5. Delete All Freshie Characters (Lvl 1, 0 XP, Maplecross, 10 Rai)");
            Console.WriteLine("  6. Delete All Dead Players");
            Console.WriteLine();
            Console.WriteLine("INFO:");
            Console.WriteLine("  7. List All Players");
            Console.WriteLine("  8. List All Dead Players");
            Console.WriteLine("  9. Check if Player Exists");
            Console.WriteLine();
            Console.WriteLine("CHARACTER MOD:");
            Console.WriteLine("  10. Character Mod Menu");
            Console.WriteLine();
            Console.WriteLine("  0. Exit");
            Console.WriteLine();
            Console.Write("Choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    migration.DryRun();
                    PressAnyKey();
                    break;

                case "2":
                    migration.MigrateAllPlayers();
                    PressAnyKey();
                    break;

                case "3":
                    Console.Write("\nEnter player name: ");
                    var playerName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(playerName))
                    {
                        migration.MigrateSinglePlayer(playerName);
                    }
                    PressAnyKey();
                    break;

                case "4":
                    DeleteSinglePlayer(db);
                    PressAnyKey();
                    break;

                case "5":
                    DeleteAllFreshies(db, baseUrl);
                    PressAnyKey();
                    break;

                case "6":
                    DeleteAllDeadPlayers(db);
                    PressAnyKey();
                    break;

                case "7":
                    ListAllPlayers(baseUrl);
                    PressAnyKey();
                    break;

                case "8":
                    ListAllDeadPlayers(db);
                    PressAnyKey();
                    break;

                case "9":
                    CheckPlayerExists(db);
                    PressAnyKey();
                    break;

                case "0":
                    Console.WriteLine("\nExiting admin tools...");
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static void DeleteSinglePlayer(PlayerDatabase db)
    {
        Console.Write("\nEnter player name to delete: ");
        var playerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Invalid player name.");
            return;
        }

        Console.Write($"\nAre you SURE you want to delete '{playerName}'? (type YES to confirm): ");
        var confirm = Console.ReadLine();

        if (confirm != "YES")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        try
        {
            var baseUrl = Environment.GetEnvironmentVariable("TBS_API_BASEURL") ?? "https://tbs-wlt8.onrender.com";
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";

            using var resp = http.DeleteAsync(url).GetAwaiter().GetResult();
            
            if (resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n✓ Player '{playerName}' has been deleted.");
            }
            else
            {
                Console.WriteLine($"\n✗ Failed to delete player. Status: {resp.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error deleting player: {ex.Message}");
        }
    }

    
    private static void DeleteAllFreshies(PlayerDatabase db, string baseUrl)
    {
        Console.WriteLine("\n╔════════════════════════════════════════╗");
        Console.WriteLine("║      DELETE ALL FRESHIE CHARACTERS     ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine("\nThis will delete all characters that match:");
        Console.WriteLine("  • Level 1");
        Console.WriteLine("  • 0 Experience");
        Console.WriteLine("  • Location: Maplecross");
        Console.WriteLine("  • Money: Exactly 10 Rai");
        Console.WriteLine("\nThese are brand new characters that haven't played yet.");
        Console.WriteLine();
        Console.Write("Type 'DELETE FRESHIES' to confirm: ");
        var confirm = Console.ReadLine();

        if (confirm != "DELETE FRESHIES")
        {
            Console.WriteLine("\nDeletion cancelled.");
            return;
        }

        try
        {
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var listUrl = $"{baseUrl.TrimEnd('/')}/players";
            
            // Get all player names
            using var listResp = http.GetAsync(listUrl).GetAwaiter().GetResult();
            
            if (!listResp.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n✗ Could not fetch player list. Status: {listResp.StatusCode}");
                return;
            }

            string json = listResp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var playerNames = Serializer.FromJson<System.Collections.Generic.List<string>>(json);

            if (playerNames == null || playerNames.Count == 0)
            {
                Console.WriteLine("\nNo players found.");
                return;
            }

            Console.WriteLine($"\nChecking {playerNames.Count} players...\n");
            
            int deletedCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            foreach (var playerName in playerNames)
            {
                try
                {
                    var player = db.LoadPlayer(playerName, "");
                    
                    var playerUrl = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
                    using var playerResp = http.GetAsync(playerUrl).GetAwaiter().GetResult();
                    
                    if (!playerResp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"  ✗ Could not load {playerName}");
                        errorCount++;
                        continue;
                    }

                    string playerJson = playerResp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var loadedPlayer = Serializer.FromJson<Player>(playerJson);

                    if (loadedPlayer == null)
                    {
                        Console.WriteLine($"  ✗ Could not deserialize {playerName}");
                        errorCount++;
                        continue;
                    }

                    bool isFreshie = loadedPlayer.level == 1 &&
                                    loadedPlayer.exp == 0 &&
                                    loadedPlayer.currentLocation == "Maplecross" &&
                                    loadedPlayer.money == 10;

                    if (isFreshie)
                    {
                        var deleteUrl = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
                        using var deleteResp = http.DeleteAsync(deleteUrl).GetAwaiter().GetResult();
                        
                        if (deleteResp.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"  ✓ Deleted freshie: {playerName}");
                            deletedCount++;
                        }
                        else
                        {
                            Console.WriteLine($"  ✗ Failed to delete {playerName} (Status: {deleteResp.StatusCode})");
                            errorCount++;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  - Skipped {playerName} (Lvl {loadedPlayer.level}, {loadedPlayer.exp} XP, {loadedPlayer.currentLocation}, {loadedPlayer.money} Rai)");
                        skippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Error processing {playerName}: {ex.Message}");
                    errorCount++;
                }
            }

            Console.WriteLine($"\n=== DELETION COMPLETE ===");
            Console.WriteLine($"Freshies deleted: {deletedCount}");
            Console.WriteLine($"Players skipped: {skippedCount}");
            Console.WriteLine($"Errors: {errorCount}");
            Console.WriteLine($"Total checked: {playerNames.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Deletion failed: {ex.Message}");
        }
    }

    private static void DeleteAllDeadPlayers(PlayerDatabase db)
    {
        Console.WriteLine("\n⚠️  This will delete ALL dead players from the database.");
        Console.Write("Type 'YES' to confirm: ");
        var confirm = Console.ReadLine();

        if (confirm != "YES")
        {
            Console.WriteLine("\nDeletion cancelled.");
            return;
        }

        try
        {
            var deadPlayers = db.GetAllDeadPlayerNames();

            if (deadPlayers.Count == 0)
            {
                Console.WriteLine("\nNo dead players found.");
                return;
            }

            Console.WriteLine($"\nDeleting {deadPlayers.Count} dead players...");
            
            int successCount = 0;
            int errorCount = 0;

            foreach (var playerName in deadPlayers)
            {
                try
                {
                    db.DeleteDeadPlayer(playerName);
                    Console.WriteLine($"  ✓ Deleted: {playerName}");
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Error deleting {playerName}: {ex.Message}");
                    errorCount++;
                }
            }

            Console.WriteLine($"\n=== DELETION COMPLETE ===");
            Console.WriteLine($"Successfully deleted: {successCount}");
            Console.WriteLine($"Errors: {errorCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Deletion failed: {ex.Message}");
        }
    }

    private static void ListAllPlayers(string baseUrl)
    {
        try
        {
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var url = $"{baseUrl.TrimEnd('/')}/players";
            Console.WriteLine($"\nFetching player list from: {url}...");
            
            using var resp = http.GetAsync(url).GetAwaiter().GetResult();
            
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n✗ Could not fetch player list. Status: {resp.StatusCode}");
                return;
            }

            string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var playerNames = Serializer.FromJson<System.Collections.Generic.List<string>>(json);

            if (playerNames == null || playerNames.Count == 0)
            {
                Console.WriteLine("\nNo players found.");
                return;
            }

            Console.WriteLine($"\n=== ACTIVE PLAYERS ({playerNames.Count}) ===");
            
            foreach (var name in playerNames)
            {
                try
                {
                    var playerUrl = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(name)}";
                    using var playerResp = http.GetAsync(playerUrl).GetAwaiter().GetResult();
                    
                    if (playerResp.IsSuccessStatusCode)
                    {
                        string playerJson = playerResp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var player = Serializer.FromJson<Player>(playerJson);
                        
                        if (player != null)
                        {
                            Console.WriteLine($"  • {name} (Level {player.level})");
                        }
                        else
                        {
                            Console.WriteLine($"  • {name} (Level ?)");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  • {name} (Level ?)");
                    }
                }
                catch
                {
                    Console.WriteLine($"  • {name} (Level ?)");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }
    }

    private static void CheckPlayerExists(PlayerDatabase db)
    {
        Console.Write("\nEnter player name to check: ");
        var playerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Invalid player name.");
            return;
        }

        try
        {
            bool exists = db.PlayerExists(playerName);
            
            if (exists)
            {
                Console.WriteLine($"\n✓ Player '{playerName}' EXISTS in the database.");
            }
            else
            {
                Console.WriteLine($"\n✗ Player '{playerName}' NOT FOUND in the database.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error checking player: {ex.Message}");
        }
    }

    private static void ListAllDeadPlayers(PlayerDatabase db)
    {
        try
        {
            var deadPlayers = db.GetAllDeadPlayerNames();

            if (deadPlayers.Count == 0)
            {
                Console.WriteLine("\nNo dead players found.");
                return;
            }

            Console.WriteLine($"\n=== DEAD PLAYERS ({deadPlayers.Count}) ===");
            foreach (var name in deadPlayers)
            {
                Console.WriteLine($"  • {name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }
    }

    private static void CharacterModMenu(PlayerDatabase db, string baseUrl)
    {
        Console.Write("\nEnter player name to modify: ");
        var playerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(playerName)) { Console.WriteLine("Invalid player name."); return; }
        try {
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
            Console.WriteLine($"\nLoading player: {playerName}...");
            using var resp = http.GetAsync(url).GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode) { Console.WriteLine($"\n✗ Could not load player. Status: {resp.StatusCode}"); return; }
            string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var player = Serializer.FromJson<Player>(json);
            if (player == null) { Console.WriteLine("\n✗ Failed to deserialize player data."); return; }
            Console.WriteLine($"\n✓ Loaded: {player.name} (Level {player.level})");
            Console.WriteLine("Character Mod Menu - Coming soon!");
        } catch (Exception ex) { Console.WriteLine($"\n✗ Error: {ex.Message}"); }
    }
    
    private static void PressAnyKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}

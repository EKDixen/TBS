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
            Console.WriteLine("  5. Delete All Dead Players");
            Console.WriteLine();
            Console.WriteLine("INFO:");
            Console.WriteLine("  6. List All Dead Players");
            Console.WriteLine("  7. Check if Player Exists");
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
                    DeleteAllDeadPlayers(db);
                    PressAnyKey();
                    break;

                case "6":
                    ListAllDeadPlayers(db);
                    PressAnyKey();
                    break;

                case "7":
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
            var http = new System.Net.Http.HttpClient();
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

    private static void PressAnyKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}

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
            Console.WriteLine("  11. Revive Dead Player (move to alive)");
            Console.WriteLine("  12. Kill Player (move to dead)");
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

                case "10":
                    CharacterModMenu(db, baseUrl);
                    PressAnyKey();
                    break;

                case "11":
                    ReviveDeadPlayer(db, baseUrl);
                    PressAnyKey();
                    break;

                case "12":
                    KillPlayer(db, baseUrl);
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
        
        try 
        {
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
            Console.WriteLine($"\nLoading player: {playerName}...");
            
            using var resp = http.GetAsync(url).GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode) 
            { 
                Console.WriteLine($"\n✗ Could not load player. Status: {resp.StatusCode}"); 
                return; 
            }
            
            string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var player = Serializer.FromJson<Player>(json);
            if (player == null) 
            { 
                Console.WriteLine("\n✗ Failed to deserialize player data."); 
                return; 
            }
            
            Console.WriteLine($"\n✓ Loaded: {player.name} (Level {player.level})");

            #region charactor mod menu
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine($"║  CHARACTER MOD: {player.name.PadRight(22)}║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine($"Name: {player.name}");
                Console.WriteLine($"Password: {player.password}");
                Console.WriteLine($"Level: {player.level} | EXP: {player.exp}");
                Console.WriteLine($"Class: {player.playerClass.name}");
                Console.WriteLine($"Location: {player.currentLocation}");
                Console.WriteLine($"Money: {player.money} Rai");
                Console.WriteLine();
                Console.WriteLine("CURRENT STATS (with equipment/buffs):");
                Console.WriteLine($"HP: {player.HP}/{player.maxHP}");
                Console.WriteLine($"Speed: {player.speed} | Armor: {player.armor}");
                Console.WriteLine($"Dodge: {player.dodge}% | Dodge Negation: {player.dodgeNegation}%");
                Console.WriteLine($"Crit Chance: {player.critChance}% | Crit Damage: {player.critDamage}%");
                Console.WriteLine($"Stun: {player.stun}% | Stun Negation: {player.stunNegation}%");
                Console.WriteLine($"Luck: {player.luck}");
                Console.WriteLine();
                Console.WriteLine("BASE STATS (without equipment):");
                Console.WriteLine($"Base Max HP: {player.baseMaxHP} | Base Speed: {player.baseSpeed}");
                Console.WriteLine($"Base Armor: {player.baseArmor} | Base Dodge: {player.baseDodge}%");
                Console.WriteLine($"Base Dodge Neg: {player.baseDodgeNegation}% | Base Crit: {player.baseCritChance}%");
                Console.WriteLine($"Base Crit Dmg: {player.baseCritDamage}% | Base Stun: {player.baseStun}%");
                Console.WriteLine($"Base Stun Neg: {player.baseStunNegation}% | Base Luck: {player.baseLuck}");
                Console.WriteLine();
                Console.WriteLine("MODIFY:");
                Console.WriteLine("  1. Name");
                Console.WriteLine("  2. Password");
                Console.WriteLine("  3. Level");
                Console.WriteLine("  4. Experience");
                Console.WriteLine("  5. Class");
                Console.WriteLine("  6. Location");
                Console.WriteLine("  7. Money");
                Console.WriteLine("  8. Stats (HP, Speed, Armor, etc.)");
                Console.WriteLine("  9. Items");
                Console.WriteLine("  10. Moves/Attacks");
                Console.WriteLine("  11. StatTracker");
                Console.WriteLine("  12. Companions");
                Console.WriteLine();
                Console.WriteLine("  S. Save Changes");
                Console.WriteLine("  0. Cancel (discard changes)");
                Console.WriteLine();
                Console.Write("Choice: ");
                
                var choice = Console.ReadLine()?.ToUpper();
                
                switch (choice)
                {
                    case "1":
                        ModifyName(player);
                        break;
                    case "2":
                        ModifyPassword(player);
                        break;
                    case "3":
                        ModifyLevel(player);
                        break;
                    case "4":
                        ModifyExperience(player);
                        break;
                    case "5":
                        ModifyClass(player);
                        break;
                    case "6":
                        ModifyLocation(player);
                        break;
                    case "7":
                        ModifyMoney(player);
                        break;
                    case "8":
                        ModifyStats(player);
                        break;
                    case "9":
                        ModifyItems(player);
                        break;
                    case "10":
                        ModifyMoves(player);
                        break;
                    case "11":
                        ModifyStatTracker(player);
                        break;
                    case "12":
                        ModifyCompanions(player);
                        break;
                    case "S":
                        if (SavePlayerChanges(player, baseUrl, http))
                        {
                            Console.WriteLine("\n✓ Changes saved successfully!");
                            PressAnyKey();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("\n✗ Failed to save changes.");
                            PressAnyKey();
                        }
                        break;
                    case "0":
                        Console.WriteLine("\nChanges discarded.");
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice.");
                        PressAnyKey();
                        break;
                }
            }
        } 
        catch (Exception ex) 
        { 
            Console.WriteLine($"\n✗ Error: {ex.Message}"); 
        }
    }

    private static void ModifyName(Player player)
    {
        Console.Write($"\nCurrent name: {player.name}");
        Console.Write("\nEnter new name (or press Enter to cancel): ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName))
        {
            player.name = newName;
            Console.WriteLine($"✓ Name changed to: {newName}");
        }
        PressAnyKey();
    }

    private static void ModifyPassword(Player player)
    {
        Console.Write($"\nCurrent password: {player.password}");
        Console.Write("\nEnter new password (or press Enter to cancel): ");
        var newPassword = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            player.password = newPassword;
            Console.WriteLine($"✓ Password changed to: {newPassword}");
        }
        PressAnyKey();
    }

    private static void ModifyLevel(Player player)
    {
        Console.Write($"\nCurrent level: {player.level}");
        Console.Write("\nEnter new level: ");
        if (int.TryParse(Console.ReadLine(), out int newLevel) && newLevel > 0)
        {
            player.level = newLevel;
            player.RecalculateStats();
            Console.WriteLine($"✓ Level changed to: {newLevel}");
        }
        else
        {
            Console.WriteLine("✗ Invalid level.");
        }
        PressAnyKey();
    }

    private static void ModifyExperience(Player player)
    {
        Console.Write($"\nCurrent experience: {player.exp}");
        Console.Write("\nEnter new experience: ");
        if (int.TryParse(Console.ReadLine(), out int newExp) && newExp >= 0)
        {
            player.exp = newExp;
            Console.WriteLine($"✓ Experience changed to: {newExp}");
        }
        else
        {
            Console.WriteLine("✗ Invalid experience value.");
        }
        PressAnyKey();
    }

    private static void ModifyClass(Player player)
    {
        Console.WriteLine($"\nCurrent class: {player.playerClass.name}");
        Console.WriteLine("\nAvailable classes:");
        
        // Use reflection to get all public static readonly Class fields from ClassLibrary
        var classType = typeof(ClassLibrary);
        var classFields = classType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Class))
            .ToList();
        
        var classList = new List<Class>();
        for (int i = 0; i < classFields.Count; i++)
        {
            var classObj = (Class)classFields[i].GetValue(null);
            classList.Add(classObj);
            Console.WriteLine($"  {i + 1}. {classObj.name}");
        }
        
        Console.Write("\nEnter class number (0 to cancel): ");
        
        if (int.TryParse(Console.ReadLine(), out int classChoice))
        {
            if (classChoice == 0)
            {
                Console.WriteLine("Cancelled.");
            }
            else if (classChoice > 0 && classChoice <= classList.Count)
            {
                player.playerClass = classList[classChoice - 1];
                player.RecalculateStats();
                Console.WriteLine($"✓ Class changed to: {player.playerClass.name}");
            }
            else
            {
                Console.WriteLine("✗ Invalid class choice.");
            }
        }
        else
        {
            Console.WriteLine("✗ Invalid input.");
        }
        PressAnyKey();
    }

    private static void ModifyLocation(Player player)
    {
        Console.Write($"\nCurrent location: {player.currentLocation}");
        Console.Write("\nEnter new location name (or press Enter to cancel): ");
        var newLocation = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newLocation))
        {
            player.currentLocation = newLocation;
            if (!player.knownLocationnames.Contains(newLocation))
            {
                player.knownLocationnames.Add(newLocation);
            }
            Console.WriteLine($"✓ Location changed to: {newLocation}");
        }
        PressAnyKey();
    }

    private static void ModifyMoney(Player player)
    {
        Console.Write($"\nCurrent money: {player.money} Rai");
        Console.Write("\nEnter new money amount: ");
        if (int.TryParse(Console.ReadLine(), out int newMoney) && newMoney >= 0)
        {
            player.money = newMoney;
            Console.WriteLine($"✓ Money changed to: {newMoney} Rai");
        }
        else
        {
            Console.WriteLine("✗ Invalid money amount.");
        }
        PressAnyKey();
    }

    private static void ModifyStats(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║          MODIFY STATS                  ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"1. HP: {player.HP} (Max: {player.maxHP})");
            Console.WriteLine($"2. Max HP (Base): {player.baseMaxHP}");
            Console.WriteLine($"3. Speed (Base): {player.baseSpeed}");
            Console.WriteLine($"4. Armor (Base): {player.baseArmor}");
            Console.WriteLine($"5. Dodge (Base): {player.baseDodge}%");
            Console.WriteLine($"6. Dodge Negation (Base): {player.baseDodgeNegation}%");
            Console.WriteLine($"7. Crit Chance (Base): {player.baseCritChance}%");
            Console.WriteLine($"8. Crit Damage (Base): {player.baseCritDamage}%");
            Console.WriteLine($"9. Stun (Base): {player.baseStun}%");
            Console.WriteLine($"10. Stun Negation (Base): {player.baseStunNegation}%");
            Console.WriteLine($"11. Luck (Base): {player.baseLuck}");
            Console.WriteLine();
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.Write($"\nCurrent HP: {player.HP}");
                    Console.Write("\nEnter new HP: ");
                    if (int.TryParse(Console.ReadLine(), out int newHP) && newHP >= 0)
                    {
                        player.HP = newHP;
                        Console.WriteLine($"✓ HP changed to: {newHP}");
                        PressAnyKey();
                    }
                    break;
                case "2":
                    Console.Write($"\nCurrent Base Max HP: {player.baseMaxHP}");
                    Console.Write("\nEnter new Base Max HP: ");
                    if (int.TryParse(Console.ReadLine(), out int newMaxHP) && newMaxHP > 0)
                    {
                        player.baseMaxHP = newMaxHP;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Max HP changed to: {newMaxHP}");
                        PressAnyKey();
                    }
                    break;
                case "3":
                    Console.Write($"\nCurrent Base Speed: {player.baseSpeed}");
                    Console.Write("\nEnter new Base Speed: ");
                    if (int.TryParse(Console.ReadLine(), out int newSpeed))
                    {
                        player.baseSpeed = newSpeed;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Speed changed to: {newSpeed}");
                        PressAnyKey();
                    }
                    break;
                case "4":
                    Console.Write($"\nCurrent Base Armor: {player.baseArmor}");
                    Console.Write("\nEnter new Base Armor: ");
                    if (int.TryParse(Console.ReadLine(), out int newArmor))
                    {
                        player.baseArmor = newArmor;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Armor changed to: {newArmor}");
                        PressAnyKey();
                    }
                    break;
                case "5":
                    Console.Write($"\nCurrent Base Dodge: {player.baseDodge}%");
                    Console.Write("\nEnter new Base Dodge: ");
                    if (int.TryParse(Console.ReadLine(), out int newDodge))
                    {
                        player.baseDodge = newDodge;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Dodge changed to: {newDodge}%");
                        PressAnyKey();
                    }
                    break;
                case "6":
                    Console.Write($"\nCurrent Base Dodge Negation: {player.baseDodgeNegation}%");
                    Console.Write("\nEnter new Base Dodge Negation: ");
                    if (int.TryParse(Console.ReadLine(), out int newDodgeNeg))
                    {
                        player.baseDodgeNegation = newDodgeNeg;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Dodge Negation changed to: {newDodgeNeg}%");
                        PressAnyKey();
                    }
                    break;
                case "7":
                    Console.Write($"\nCurrent Base Crit Chance: {player.baseCritChance}%");
                    Console.Write("\nEnter new Base Crit Chance: ");
                    if (int.TryParse(Console.ReadLine(), out int newCrit))
                    {
                        player.baseCritChance = newCrit;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Crit Chance changed to: {newCrit}%");
                        PressAnyKey();
                    }
                    break;
                case "8":
                    Console.Write($"\nCurrent Base Crit Damage: {player.baseCritDamage}%");
                    Console.Write("\nEnter new Base Crit Damage: ");
                    if (int.TryParse(Console.ReadLine(), out int newCritDmg))
                    {
                        player.baseCritDamage = newCritDmg;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Crit Damage changed to: {newCritDmg}%");
                        PressAnyKey();
                    }
                    break;
                case "9":
                    Console.Write($"\nCurrent Base Stun: {player.baseStun}%");
                    Console.Write("\nEnter new Base Stun: ");
                    if (int.TryParse(Console.ReadLine(), out int newStun))
                    {
                        player.baseStun = newStun;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Stun changed to: {newStun}%");
                        PressAnyKey();
                    }
                    break;
                case "10":
                    Console.Write($"\nCurrent Base Stun Negation: {player.baseStunNegation}%");
                    Console.Write("\nEnter new Base Stun Negation: ");
                    if (int.TryParse(Console.ReadLine(), out int newStunNeg))
                    {
                        player.baseStunNegation = newStunNeg;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Stun Negation changed to: {newStunNeg}%");
                        PressAnyKey();
                    }
                    break;
                case "11":
                    Console.Write($"\nCurrent Base Luck: {player.baseLuck}");
                    Console.Write("\nEnter new Base Luck: ");
                    if (int.TryParse(Console.ReadLine(), out int newLuck))
                    {
                        player.baseLuck = newLuck;
                        player.RecalculateStats();
                        Console.WriteLine($"✓ Base Luck changed to: {newLuck}");
                        PressAnyKey();
                    }
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid choice.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static void ModifyItems(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║          MODIFY ITEMS                  ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Owned Items: {player.ownedItems.Count}");
            Console.WriteLine($"Equipped Items: {player.equippedItems.Count(i => i != null)}");
            Console.WriteLine($"Material Items: {player.materialItems.Count}");
            Console.WriteLine($"Bank Items: {player.bankItems.Count}");
            Console.WriteLine();
            Console.WriteLine("1. View Owned Items");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. Clear All Items");
            Console.WriteLine("5. Clear Inventory");
            Console.WriteLine("6. Clear Material Bag");
            Console.WriteLine("7. Clear Bank");
            Console.WriteLine();
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n--- Owned Items ---");
                    if (player.ownedItems.Count == 0)
                    {
                        Console.WriteLine("No items.");
                    }
                    else
                    {
                        for (int i = 0; i < player.ownedItems.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {player.ownedItems[i].name} (x{player.ownedItems[i].amount})");
                        }
                    }
                    PressAnyKey();
                    break;
                case "2":
                    AddItemToPlayer(player);
                    break;
                case "3":
                    RemoveItemFromPlayer(player);
                    break;
                case "4":
                    Console.Write("\nAre you sure you want to clear ALL items? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.ownedItems.Clear();
                        player.equippedItems.Clear();
                        player.materialItems.Clear();
                        player.bankItems.Clear();
                        player.equippedWeapon = null;
                        Console.WriteLine("✓ All items cleared.");
                    }
                    PressAnyKey();
                    break;
                case "5":
                    Console.Write("\nAre you sure you want to clear inventory? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.ownedItems.Clear();
                        player.equippedItems.Clear();
                        player.equippedWeapon = null;
                        Console.WriteLine("✓ Inventory cleared.");
                    }
                    PressAnyKey();
                    break;
                case "6":
                    Console.Write("\nAre you sure you want to clear material bag? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.materialItems.Clear();
                        player.currentMaterialLoad = 0;
                        Console.WriteLine("✓ Material bag cleared.");
                    }
                    PressAnyKey();
                    break;
                case "7":
                    Console.Write("\nAre you sure you want to clear bank? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.bankItems.Clear();
                        player.bankMoney = 0;
                        Console.WriteLine("✓ Bank cleared.");
                    }
                    PressAnyKey();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid choice.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static void AddItemToPlayer(Player player)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║            ADD ITEM                    ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        
        // Use reflection to get all public static Item fields from ItemLibrary
        var itemLibraryType = typeof(ItemLibrary);
        var itemFields = itemLibraryType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Item))
            .ToList();
        
        var itemList = new List<Item>();
        Console.WriteLine("Available Items:");
        Console.WriteLine();
        
        for (int i = 0; i < itemFields.Count; i++)
        {
            var item = (Item)itemFields[i].GetValue(null);
            itemList.Add(item);
            
            string typeInfo = item.type.ToString();
            if (item.type == ItemType.equipment)
            {
                typeInfo = $"{item.equipmentType} {item.type}";
            }
            
            Console.WriteLine($"  {i + 1}. {item.name} ({typeInfo})");
        }
        
        Console.WriteLine();
        Console.Write("Enter item number (0 to cancel): ");
        
        if (int.TryParse(Console.ReadLine(), out int itemChoice))
        {
            if (itemChoice == 0)
            {
                return;
            }
            else if (itemChoice > 0 && itemChoice <= itemList.Count)
            {
                var selectedItem = itemList[itemChoice - 1];
                var itemToAdd = new Item(selectedItem);
                
                Console.Write("\nEnter quantity (default 1): ");
                if (int.TryParse(Console.ReadLine(), out int qty) && qty > 0)
                {
                    itemToAdd.amount = qty;
                }
                else
                {
                    itemToAdd.amount = 1;
                }
                
                if (itemToAdd.type == ItemType.material)
                {
                    player.materialItems.Add(itemToAdd);
                }
                else
                {
                    player.ownedItems.Add(itemToAdd);
                }
                
                Console.WriteLine($"✓ Added {itemToAdd.amount}x {itemToAdd.name}");
            }
            else
            {
                Console.WriteLine("✗ Invalid selection.");
            }
        }
        else
        {
            Console.WriteLine("✗ Invalid input.");
        }
        
        PressAnyKey();
    }

    private static void RemoveItemFromPlayer(Player player)
    {
        Console.WriteLine("\n--- Remove Item ---");
        Console.WriteLine("1. From Inventory");
        Console.WriteLine("2. From Material Bag");
        Console.Write("\nChoice: ");
        
        var choice = Console.ReadLine();
        List<Item> targetList = choice == "2" ? player.materialItems : player.ownedItems;
        
        if (targetList.Count == 0)
        {
            Console.WriteLine("No items to remove.");
            PressAnyKey();
            return;
        }
        
        Console.WriteLine("\nItems:");
        for (int i = 0; i < targetList.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {targetList[i].name} (x{targetList[i].amount})");
        }
        
        Console.Write("\nEnter item number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int itemNum) && itemNum > 0 && itemNum <= targetList.Count)
        {
            var item = targetList[itemNum - 1];
            targetList.RemoveAt(itemNum - 1);
            Console.WriteLine($"✓ Removed {item.name}");
        }
        else
        {
            Console.WriteLine("✗ Invalid selection.");
        }
        
        PressAnyKey();
    }

    private static void ModifyMoves(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║          MODIFY MOVES                  ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Owned Attacks: {player.ownedAttacks.Count}");
            Console.WriteLine($"Equipped Attacks: {player.equippedAttacks.Count(a => a != null)}");
            Console.WriteLine();
            Console.WriteLine("1. View Owned Attacks");
            Console.WriteLine("2. Add Attack");
            Console.WriteLine("3. Remove Attack");
            Console.WriteLine("4. Clear All Attacks");
            Console.WriteLine();
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n--- Owned Attacks ---");
                    if (player.ownedAttacks.Count == 0)
                    {
                        Console.WriteLine("No attacks.");
                    }
                    else
                    {
                        for (int i = 0; i < player.ownedAttacks.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {player.ownedAttacks[i].name}");
                        }
                    }
                    PressAnyKey();
                    break;
                case "2":
                    AddAttackToPlayer(player);
                    break;
                case "3":
                    RemoveAttackFromPlayer(player);
                    break;
                case "4":
                    Console.Write("\nAre you sure you want to clear all attacks? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.ownedAttacks.Clear();
                        player.equippedAttacks.Clear();
                        Console.WriteLine("✓ All attacks cleared.");
                    }
                    PressAnyKey();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid choice.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static void AddAttackToPlayer(Player player)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║          ADD ATTACK                    ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        
        var attackLibraryType = typeof(AttackLibrary);
        var attackFields = attackLibraryType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Attack))
            .ToList();
        
        var attackList = new List<Attack>();
        Console.WriteLine("Available Attacks:");
        Console.WriteLine();
        
        for (int i = 0; i < attackFields.Count; i++)
        {
            var attack = (Attack)attackFields[i].GetValue(null);
            attackList.Add(attack);
            
            string desc = attack.GetDescription();
            if (desc.Length > 50)
            {
                desc = desc.Substring(0, 47) + "...";
            }
            
            Console.WriteLine($"  {i + 1}. {attack.name} - {desc}");
        }
        
        Console.WriteLine();
        Console.Write("Enter attack number (0 to cancel): ");
        
        if (int.TryParse(Console.ReadLine(), out int attackChoice))
        {
            if (attackChoice == 0)
            {
                return;
            }
            else if (attackChoice > 0 && attackChoice <= attackList.Count)
            {
                var attackToAdd = attackList[attackChoice - 1];
                
                if (!player.ownedAttacks.Any(a => a.name == attackToAdd.name))
                {
                    player.ownedAttacks.Add(attackToAdd);
                    Console.WriteLine($"\n✓ Added attack: {attackToAdd.name}");
                }
                else
                {
                    Console.WriteLine($"\n✗ Player already has {attackToAdd.name}");
                }
            }
            else
            {
                Console.WriteLine("✗ Invalid selection.");
            }
        }
        else
        {
            Console.WriteLine("✗ Invalid input.");
        }
        
        PressAnyKey();
    }

    private static void RemoveAttackFromPlayer(Player player)
    {
        if (player.ownedAttacks.Count == 0)
        {
            Console.WriteLine("\nNo attacks to remove.");
            PressAnyKey();
            return;
        }
        
        Console.WriteLine("\n--- Remove Attack ---");
        for (int i = 0; i < player.ownedAttacks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {player.ownedAttacks[i].name}");
        }
        
        Console.Write("\nEnter attack number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int attackNum) && attackNum > 0 && attackNum <= player.ownedAttacks.Count)
        {
            var attack = player.ownedAttacks[attackNum - 1];
            player.ownedAttacks.RemoveAt(attackNum - 1);
            
            for (int i = 0; i < player.equippedAttacks.Count; i++)
            {
                if (player.equippedAttacks[i]?.name == attack.name)
                {
                    player.equippedAttacks[i] = null;
                }
            }
            
            Console.WriteLine($"✓ Removed {attack.name}");
        }
        else
        {
            Console.WriteLine("✗ Invalid selection.");
        }
        
        PressAnyKey();
    }

    private static void ModifyStatTracker(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       MODIFY STAT TRACKER              ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            
            if (player.statTracker.Count == 0)
            {
                Console.WriteLine("No stats tracked yet.");
            }
            else
            {
                Console.WriteLine("Current Stats:");
                foreach (var stat in player.statTracker)
                {
                    Console.WriteLine($"  {stat.Key}: {stat.Value}");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("1. Add/Modify Stat");
            Console.WriteLine("2. Remove Stat");
            Console.WriteLine("3. Clear All Stats");
            Console.WriteLine();
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.Write("\nEnter stat name: ");
                    var statName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(statName))
                    {
                        Console.Write($"Enter value for '{statName}': ");
                        if (int.TryParse(Console.ReadLine(), out int value))
                        {
                            player.SetStat(statName, value);
                            Console.WriteLine($"✓ Stat '{statName}' set to {value}");
                        }
                        else
                        {
                            Console.WriteLine("✗ Invalid value.");
                        }
                    }
                    PressAnyKey();
                    break;
                case "2":
                    Console.Write("\nEnter stat name to remove: ");
                    var statToRemove = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(statToRemove) && player.HasStat(statToRemove))
                    {
                        player.statTracker.Remove(statToRemove);
                        Console.WriteLine($"✓ Stat '{statToRemove}' removed.");
                    }
                    else
                    {
                        Console.WriteLine("✗ Stat not found.");
                    }
                    PressAnyKey();
                    break;
                case "3":
                    Console.Write("\nAre you sure you want to clear all stats? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        player.statTracker.Clear();
                        Console.WriteLine("✓ All stats cleared.");
                    }
                    PressAnyKey();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nInvalid choice.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static void ModifyCompanions(Player player)
    {
        while (true)
        {
            int maxCompanions = CompanionSystem.GetMaxCompanions(player);
            var companions = CompanionSystem.GetCompanions(player);
            
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       MODIFY COMPANIONS                ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            
            Console.WriteLine($"Max Companions: {maxCompanions}");
            Console.WriteLine($"Current Companions: {companions.Count}\n");
            
            if (companions.Count > 0)
            {
                Console.WriteLine("Current Companions:");
                for (int i = 0; i < companions.Count; i++)
                {
                    var c = companions[i];
                    string status = c.IsAlive() ? "Alive" : "Dead";
                    Console.WriteLine($"  {i + 1}. {c.name} - Lvl {c.level} - HP: {c.HP}/{c.maxHP} ({status})");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine("1. Add Companion");
            Console.WriteLine("2. Remove Companion");
            Console.WriteLine("3. Remove All Companions");
            Console.WriteLine("4. Set Max Companions");
            Console.WriteLine("5. Heal All Companions");
            Console.WriteLine();
            Console.WriteLine("0. Back");
            Console.WriteLine();
            Console.Write("Choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.Write("\nEnter companion name (e.g., goblin, thug, dire wolf): ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        bool success = CompanionSystem.RecruitByName(player, name);
                        if (!success)
                        {
                            Console.WriteLine("\n✗ Failed to add companion.");
                        }
                    }
                    PressAnyKey();
                    break;
                    
                case "2":
                    if (companions.Count == 0)
                    {
                        Console.WriteLine("\nNo companions to remove.");
                    }
                    else
                    {
                        Console.Write("\nEnter companion number to remove: ");
                        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= companions.Count)
                        {
                            bool success = CompanionSystem.RemoveCompanion(player, index - 1);
                            if (success)
                            {
                                Console.WriteLine("\n✓ Companion removed!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("✗ Invalid selection.");
                        }
                    }
                    PressAnyKey();
                    break;
                    
                case "3":
                    Console.Write("\nAre you sure you want to remove all companions? (type YES): ");
                    if (Console.ReadLine() == "YES")
                    {
                        CompanionSystem.DismissAllCompanions(player);
                        Console.WriteLine("✓ All companions removed!");
                    }
                    PressAnyKey();
                    break;
                    
                case "4":
                    Console.Write($"\nCurrent max companions: {maxCompanions}");
                    Console.Write("\nEnter new max companions: ");
                    if (int.TryParse(Console.ReadLine(), out int newMax) && newMax >= 0)
                    {
                        CompanionSystem.SetMaxCompanions(player, newMax);
                        Console.WriteLine($"✓ Max companions set to: {newMax}");
                    }
                    else
                    {
                        Console.WriteLine("✗ Invalid number.");
                    }
                    PressAnyKey();
                    break;
                    
                case "5":
                    CompanionSystem.HealAllCompanions(player);
                    PressAnyKey();
                    break;
                    
                case "0":
                    return;
                    
                default:
                    Console.WriteLine("\nInvalid choice.");
                    PressAnyKey();
                    break;
            }
        }
    }

    private static bool SavePlayerChanges(Player player, string baseUrl, System.Net.Http.HttpClient http)
    {
        try
        {
            Console.WriteLine("\nSaving changes...");
            
            var json = Serializer.ToJson(player);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(player.name)}";
            
            using var resp = http.PutAsync(url, content).GetAwaiter().GetResult();
            
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving: {ex.Message}");
            return false;
        }
    }
    
    private static void ReviveDeadPlayer(PlayerDatabase db, string baseUrl)
    {
        Console.Write("\nEnter dead player name to revive: ");
        var playerName = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Invalid player name.");
            return;
        }
        
        try
        {
            var deadPlayer = db.LoadDeadPlayer(playerName);
            
            if (deadPlayer == null)
            {
                Console.WriteLine($"\n✗ Dead player '{playerName}' not found.");
                return;
            }
            
            Console.WriteLine($"\n✓ Found dead player: {deadPlayer.name} (Level {deadPlayer.level})");
            Console.Write("\nAre you sure you want to revive this player? (type YES): ");
            
            if (Console.ReadLine() != "YES")
            {
                Console.WriteLine("Revive cancelled.");
                return;
            }
            
            deadPlayer.isDead = false;
            
            deadPlayer.HP = deadPlayer.maxHP;
            
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            
            var json = Serializer.ToJson(deadPlayer);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
            
            using var resp = http.PutAsync(url, content).GetAwaiter().GetResult();
            
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n✗ Failed to save revived player. Status: {resp.StatusCode}");
                return;
            }
            
            db.DeleteDeadPlayer(playerName);
            
            Console.WriteLine($"\n✓ Player '{playerName}' has been revived and moved to active players!");
            Console.WriteLine($"   HP restored to {deadPlayer.HP}/{deadPlayer.maxHP}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error reviving player: {ex.Message}");
        }
    }
    #endregion
    private static void KillPlayer(PlayerDatabase db, string baseUrl)
    {
        Console.Write("\nEnter player name to kill: ");
        var playerName = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Invalid player name.");
            return;
        }
        
        try
        {
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var http = new System.Net.Http.HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(30);
            var url = $"{baseUrl.TrimEnd('/')}/players/{Uri.EscapeDataString(playerName)}";
            
            using var resp = http.GetAsync(url).GetAwaiter().GetResult();
            
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"\n✗ Player '{playerName}' not found.");
                return;
            }
            
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n✗ Could not load player. Status: {resp.StatusCode}");
                return;
            }
            
            string json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var player = Serializer.FromJson<Player>(json);
            
            if (player == null)
            {
                Console.WriteLine("\n✗ Failed to deserialize player data.");
                return;
            }
            
            Console.WriteLine($"\n✓ Found player: {player.name} (Level {player.level})");
            Console.Write("\nAre you SURE you want to kill this player? (type YES): ");
            
            if (Console.ReadLine() != "YES")
            {
                Console.WriteLine("Kill cancelled.");
                return;
            }
            
            db.MarkPlayerAsDead(player);
            
            Console.WriteLine($"\n✓ Player '{playerName}' has been killed and moved to dead players.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error killing player: {ex.Message}");
        }
    }

    private static void PressAnyKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}

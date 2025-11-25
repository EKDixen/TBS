using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Game.Class
{
    public class Program
    {
        private const string CURRENT_VERSION = "1.0.22"; // Update this with each release
        private const string GITHUB_API_URL = "https://api.github.com/repos/EKDixen/TBS/releases/latest";

        static bool stopMultibleLoad = false;
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static AttackManager atkManager;
        static Encyclopedia Encyclopedia;

        public static Player? pendingDeadPlayerUpdate = null;
        public static Enemy? pendingSpiritEnemy = null;

        public static void Main(string[] args)
        {
            // Check for updates before starting the game
            CheckForUpdates();

            // Lock console window at startup (prevents resizing, which fucks up the UI)
            var ui = new CombatUI();
            ui.InitializeConsole();

            while (true)
            {
                Console.WriteLine("Welcome! Do you want to:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create a new character");

                string choice = Console.ReadKey(true).KeyChar.ToString();

                if (choice == "1")
                {
                    Console.Write("\nEnter username: ");
                    string username = Console.ReadLine();

                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();

                    player = db.LoadPlayer(username, password);

                    if (player != null)
                    {
                        if (player.isDead)
                        {
                            ShowDeadCharacterScreen(player);
                            continue;
                        }

                        Console.WriteLine($"Welcome back, {player.name} (Level {player.level})!");
                        Inventory.MakeInv();
                        break;
                    }
                    else
                    {
                        var deadPlayer = db.LoadDeadPlayer(username);
                        if (deadPlayer != null && deadPlayer.password == password)
                        {
                            ShowDeadCharacterScreen(deadPlayer);
                            continue;
                        }

                        Console.WriteLine("\nInvalid username or password.");
                        continue;
                    }
                }
                else if (choice == "2")
                {
                    const int MaxUsernameLength = 12;
                    string name;
                    bool isValidUsername;

                    Console.WriteLine("\n--- Character Creation ---");
                    Console.WriteLine($"Username Rules: Max {MaxUsernameLength} characters, no spaces.");

                    do
                    {
                        Console.WriteLine("\nName your character (needed to login):");
                        name = Console.ReadLine();

                        isValidUsername = (name.Length <= MaxUsernameLength && !name.Contains(' ') && name != null);

                        if (!isValidUsername)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (name.Length > MaxUsernameLength)
                            {
                                Console.WriteLine($"Error: Username is too long. Must be {MaxUsernameLength} characters or less.");
                            }
                            else if (name.Contains(' '))
                            {
                                Console.WriteLine("Error: Username cannot contain spaces.");
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid username format. Please try again.");
                            }
                            Console.ResetColor();
                        }
                        else if (db.PlayerExists(name))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("A player with that name already exists, please try again");
                            Console.ResetColor();
                            isValidUsername = false;
                        }

                    } while (!isValidUsername);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Username accepted!");
                    Console.ResetColor();

                    PlayerCreator creator = new PlayerCreator();
                    player = creator.PlayerCreatorFunction(db, name);
                    db.SavePlayer(player);
                    Inventory.MakeInv();
                    Inventory.AddItem(ItemLibrary.rock, 1);
                    Console.WriteLine("New character created and saved!");
                    break;
                }
                else
                {
                    Console.WriteLine("\nInvalid choice, write 1 or 2 please.");
                    continue;
                }
            }
            atkManager = new AttackManager(player);
            MainUI.InitializeConsole();
            CheckPlayerLevel();
            MainMenu();
        }

        private static void CheckForUpdates()
        {
            try
            {
                Console.WriteLine("Checking for updates...");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TBS-Game");
                    client.Timeout = TimeSpan.FromSeconds(5);

                    var response = client.GetStringAsync(GITHUB_API_URL).Result;
                    JObject json = JObject.Parse(response);
                    string latestVersion = json["tag_name"]?.ToString()?.TrimStart('v') ?? "";

                    if (!string.IsNullOrEmpty(latestVersion) && latestVersion != CURRENT_VERSION)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("╔════════════════════════════════════════╗");
                        Console.WriteLine("║                                        ║");
                        Console.WriteLine("║          OUTDATED VERSION!             ║");
                        Console.WriteLine("║                                        ║");
                        Console.WriteLine("╚════════════════════════════════════════╝");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine($"Current version: {CURRENT_VERSION}");
                        Console.WriteLine($"Latest version:  {latestVersion}");
                        Console.WriteLine();
                        Console.WriteLine("This version is outdated and cannot be used.");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Please use TBSLauncher.exe to download the latest version.");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to close...");
                        Console.ReadKey(true);

                        // Close the game
                        Environment.Exit(0);
                    }
                    else if (!string.IsNullOrEmpty(latestVersion))
                    {
                        Console.WriteLine($"Game is up to date (v{CURRENT_VERSION})");
                        Thread.Sleep(800);
                        Console.Clear();
                    }
                }
            }
            catch
            {
                // Silently fail if can't check for updates (no internet, etc.)
                // Game will continue normally
            }
        }

        public static void MainMenu()
        {

            MainUI.ShowMovesInPlayerPanel = false;

            Console.Clear(); //do not remove
            MainUI.ClearMainArea();

            MainUI.RenderMainMenuScreen(player);
            if (!stopMultibleLoad)
            {
                MainUI.LoopRenderMain();
                stopMultibleLoad = true;
            }


            // Save in background without blocking UI
            _ = Task.Run(() => db.SavePlayer(player));

            MainUI.WriteInMainArea("What do you wish to do? (type the number next to it)");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Go somewhere : 0");
            MainUI.WriteInMainArea("Check Inventory : 1");
            MainUI.WriteInMainArea("Check Moves : 2");
            MainUI.WriteInMainArea($"Do something at {player.currentLocation} : 3");
            MainUI.WriteInMainArea("Check stats : 4");
            //MainUI.WriteInMainArea("Check Encyclopedia : 6");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 5 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, 4, or 5");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                MainMenu();
                return;
            }
            else if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1) 
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("Inventory : 0");
                MainUI.WriteInMainArea("MaterialBag : 1");

                if(int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input2) == false || input2 > 1 || input2 < 0)
                {
                    MainUI.WriteInMainArea("\nyou gotta type 0 or 1");
                    MainUI.WriteInMainArea("Press enter to continue...");
                    Console.ReadLine();
                    MainMenu();
                    return;
                }
                else if (input2 == 0)
                {
                    Inventory.ShowInventory();
                }
                else if (input2 == 1)
                {
                    Inventory.ShowMaterialBag();
                }
        }
            else if (input == 2) atkManager.ShowMovesMenu();
            else if (input == 3)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("all establisments in your current location\n");
                int i = 0;
                foreach (var subLocation in LocationLibrary.Get(player.currentLocation).subLocationsHere)
                {
                    i++;
                    MainUI.WriteInMainArea($"{subLocation.name} : {i}");
                }
                if (i == 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("there arent any establisments in your current location sorry");
                    MainMenu();
                    return;
                }
                MainUI.WriteInMainArea("Cancel : 0");
                MainUI.WriteInMainArea("\ntype out the number next to the location you want to go to\n");

                int targetDes;
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out targetDes))
                {
                    if (targetDes == 0)
                    {
                        MainUI.ClearMainArea();
                        MainMenu();
                        return;
                    }
                    else if (targetDes > LocationLibrary.Get(player.currentLocation).subLocationsHere.Count || targetDes < 0)
                    {
                        MainUI.WriteInMainArea("that number is wrong mate");
                        MainMenu();
                        return;
                    }
                    LocationLibrary.Get(player.currentLocation).subLocationsHere[targetDes - 1].DoSubLocation();
                }
                else
                {
                    MainUI.WriteInMainArea("write a number dumb dumb");
                    MainMenu();
                    return;
                }
            }
            else if (input == 4) ShowPlayerStats();

            // Save in background without blocking UI
            _ = Task.Run(() => db.SavePlayer(player));
        }

        public static void ShowPlayerStats()
        {
            MainUI.ClearMainArea();

            MainUI.WriteInMainArea($"\nAccount Name: {player.name} \n\n1 : Level: {player.level} \n2 : Class: {player.playerClass.name} \n3 : HP: {player.HP}/{player.maxHP} \n4 : Speed: {player.speed} \n5 : armor: {player.armor}" +
                $"\n6 : Dodge: {player.dodge}% \n7 : DodgeNegation: {player.dodgeNegation}% \n8 : Crit-chance: {player.critChance}% \n9 : Crit-Damage: {player.critDamage}% \n 10 : Stun: {player.stun}%" +
                $"\n11 : StunNegation: {player.stunNegation}%\n\n");

            Thread.Sleep(400);
            MainUI.WriteInMainArea("0 : Cancel");
            MainUI.WriteInMainArea("Press any stat's corresponding number for details about it");
            string st = Console.ReadLine();

            if (int.TryParse(st, out int input) == false || input > 11 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea(" \nyou gotta type a real number:)");

                MainUI.WriteInMainArea(" \npress enter to continue...");
                Console.ReadLine();
                ShowPlayerStats();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                switch (input)
                {
                    case 0:
                        MainMenu();
                        return;
                    case 1:
                        MainUI.WriteInMainArea("For each levelup you gain:");
                        MainUI.WriteInMainArea($"    {player.playerClass.TmaxHP} maxHP");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tspeed} speed");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tarmor} armor");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tdodge} dodge");
                        MainUI.WriteInMainArea($"    {player.playerClass.TdodgeNegation} dodgeNegation");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tcritchance} critChance");
                        MainUI.WriteInMainArea($"    {player.playerClass.TcritDamage} critDamage");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tstun} stun");
                        MainUI.WriteInMainArea($"    {player.playerClass.TstunNegation} stunNegation");
                        break;
                    case 2:
                        MainUI.WriteInMainArea($"your class is{player.playerClass.name}");
                        MainUI.WriteInMainArea($"{player.playerClass.description}");
                        break;
                    case 3:
                        MainUI.WriteInMainArea("Your hp stat is how much you can get hit before you die");
                        break;
                    case 4:
                        MainUI.WriteInMainArea("Your speed stat determines how often you get to take actions in combat");
                        break;
                    case 5:
                        MainUI.WriteInMainArea("Your armor stat acts as a flat decrease to damage taken during combat");
                        break;
                    case 6:
                        MainUI.WriteInMainArea("Your dodge stat gives you a chance to avoid enemy attacks entirely");
                        break;
                    case 7:
                        MainUI.WriteInMainArea("Your dodgeNegation stat makes it harder for your enemies to dodge your \n attacks");
                        break;
                    case 8:
                        MainUI.WriteInMainArea("Your critChance stat gives you a chance to deal bonus damage on your \n attacks");
                        break;
                    case 9:
                        MainUI.WriteInMainArea("Your critDamage stat determines how much extra damage you deal when you crit");
                        break;
                    case 10:
                        MainUI.WriteInMainArea("Your stun stat gives you a chance to stun your opponents when you hit them \n with an attack");
                        break;
                    case 11:
                        MainUI.WriteInMainArea("Your stunNegation stat makes it harder for your opponent to stun you");
                        break;
                }

                MainUI.WriteInMainArea(" \nPress enter to continue...");
                Console.ReadLine();
                ShowPlayerStats();
            }
        }

        public static async Task SavePlayer()
        {
            db.SavePlayer(player);
        }
        public static async Task CheckPlayerLevel()
        {
            while (true)
            {
                if (player.exp >= player.level * 100)
                {
                    player.level++;
                    player.exp = 0;

                    // Recalculate all derived stats based on current class & level
                    player.RecalculateStats();
                }

                await Task.Delay(3200);
            }
        }

        public static void CheckPlayerDeath()
        {
            if (player != null && player.HP <= 0)
            {
                if (pendingDeadPlayerUpdate != null && pendingSpiritEnemy != null)
                {
                    try
                    {
                        pendingDeadPlayerUpdate.HP = pendingSpiritEnemy.HP;
                        db.UpdateDeadPlayer(pendingDeadPlayerUpdate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating spirit HP: {ex.Message}");
                    }
                    finally
                    {
                        pendingDeadPlayerUpdate = null;
                        pendingSpiritEnemy = null;
                    }
                }

                ShowDeathScreen();
            }
        }

        private static void ShowDeathScreen()
        {
            try
            {
                db.MarkPlayerAsDead(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving dead player: {ex.Message}");
            }

            Console.Clear();
            Console.CursorVisible = true;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║          YOU HAVE DIED                 ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine($"Character: {player.name}");
            Console.WriteLine($"Level: {player.level}");
            Console.WriteLine($"Class: {player.playerClass.name}");
            Console.WriteLine("");
            Console.WriteLine("Your character has died...");
            Console.WriteLine("They have been moved to the realm of the dead.");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to close the game...");

            try
            {
                Console.ReadLine();
            }
            catch { }

            player = null;

            Console.Clear();
            Console.WriteLine("Game closing...");
            Thread.Sleep(1000);

            // Close the game
            Environment.Exit(0);
        }

        private static void ShowDeadCharacterScreen(Player deadPlayer)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║       THIS CHARACTER IS DEAD           ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine($"Character: {deadPlayer.name}");
            Console.WriteLine($"Level: {deadPlayer.level}");
            Console.WriteLine($"Class: {deadPlayer.playerClass.name}");
            Console.WriteLine("");
            Console.WriteLine("This character died and can no longer be played.");
            Console.WriteLine("Their spirit lingers in the realm of the dead...");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to return to login...");

            Console.ReadLine();
        }
    }
}
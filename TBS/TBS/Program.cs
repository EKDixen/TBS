using System.Drawing;

namespace Game.Class
{
    public class Program
    {
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static Inventory Inventory;
        static AttackManager atkManager;

        public static void Main(string[] args)
        {
            // Lock console window at startup (prevents resizing, which fucks up the UI)
            var ui = new CombatUI();
            ui.InitializeConsole();
            
            while (true)
            {
                Console.WriteLine("Welcome! Do you want to:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create a new character");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Enter username: ");
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
                    Console.WriteLine("Name your character (needed to login)");
                    string name = Console.ReadLine();

                    if (db.PlayerExists(name))
                    {
                        Console.WriteLine("A player with that name already exists, please try again");
                        continue;
                    }

                    PlayerCreator creator = new PlayerCreator();
                    player = creator.PlayerCreatorFunction(db,name);
                    db.SavePlayer(player);
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
            Inventory = new Inventory(player);
            MainUI.InitializeConsole();
            MainMenu();
            CheckPlayerLevel();
            MainUI.LoopRenderMain();
        }

        public static void MainMenu()
        {

            Console.Clear(); //do not remove (ima remove it) . nigga du dum 
            MainUI.ClearMainArea();

            MainUI.RenderMainMenuScreen(player);


            db.SavePlayer(player);

            MainUI.WriteInMainArea("What do you wish to do? (type the number next to it)");
            MainUI.WriteInMainArea(""); 
            MainUI.WriteInMainArea("Go somewhere : 0");
            MainUI.WriteInMainArea("Check Inventory : 1");
            MainUI.WriteInMainArea("Check Moves : 2");
            MainUI.WriteInMainArea($"Do something at {player.currentLocation.name} : 3");
            MainUI.WriteInMainArea("Check stats : 4");

            if (int.TryParse(Console.ReadLine(), out int input) == false || input > 4 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, or 4");
                MainMenu();
                return;
            }
            else if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1) Inventory.ShowInventory();
            else if (input == 2) atkManager.ShowMovesMenu();
            else if (input == 3)
            {
                MainUI.WriteInMainArea("all establisments in your current location");
                int i = 0;
                foreach (var subLocation in player.currentLocation.subLocationsHere)
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

                MainUI.WriteInMainArea("\ntype out the number next to the location you want to go to\n or leave : 0");

                int targetDes;
                if (int.TryParse(Console.ReadLine(), out targetDes))
                {
                    if (targetDes == 0)
                    {
                        MainUI.ClearMainArea();
                        MainMenu();
                        return;
                    }
                    else if (targetDes > player.currentLocation.subLocationsHere.Count || targetDes < 0)
                    {
                        MainUI.WriteInMainArea("that number is wrong mate");
                        MainMenu();
                        return;
                    }
                    player.currentLocation.subLocationsHere[targetDes - 1].DoSubLocation();
                }
                else
                {
                    MainUI.WriteInMainArea("write a number dumb dumb");
                    MainMenu();
                    return;
                }
            }
            else if (input == 4) ShowPlayerStats();

            db.SavePlayer(player);
        }

        public static void ShowPlayerStats()
        {
            MainUI.ClearMainArea();

            MainUI.WriteInMainArea($"\nAccount Name: {player.name} \n\nLevel: {player.level} \nClass: {player.playerClass} \nHP: {player.HP}/{player.maxHP} \nDMG: {player.DMG} \nSpeed: {player.speed} \narmor: {player.armor}" +
                $"\nDodge: {player.dodge} \nDodgeNegation: {player.dodgeNegation} \nCrit-chance: {player.critChance} \nCrit-Damage: {player.critDamage} \nStun: {player.stun}" +
                $"\nStunNegation: {player.stunNegation}\n\n");

            Thread.Sleep(400);
            MainUI.WriteInMainArea("-press Enter to continue");

            Console.ReadLine();

            MainMenu();
        }

        public static void SavePlayer()
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

                    //missing function to do the effects of level up

                }

                await Task.Delay(3200);
            }
        }

        public static void CheckPlayerDeath()
        {
            if (player != null && player.HP <= 0)
            {
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
            Console.WriteLine($"Class: {player.playerClass}");
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
            Console.WriteLine($"Class: {deadPlayer.playerClass}");
            Console.WriteLine("");
            Console.WriteLine("This character died and can no longer be played.");
            Console.WriteLine("Their spirit lingers in the realm of the dead...");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to return to login...");
            
            Console.ReadLine();
        }
    }
}

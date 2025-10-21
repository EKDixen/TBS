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
        static Random rng = new Random();

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
                        Console.WriteLine($"Welcome back, {player.name} (Level {player.level})!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid username or password.");
                        continue;
                    }
                }
                else if (choice == "2")
                {
                    PlayerCreator creator = new PlayerCreator();
                    player = creator.PlayerCreatorFunction(db);
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


        }

        public static void MainMenu()
        {

            Console.Clear(); //do not remove 
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
            MainUI.WriteInMainArea(""); 
            MainUI.WriteInMainArea("Start test combat (1v1) : 5");
            MainUI.WriteInMainArea("Start test combat (1v2) : 6");
            MainUI.WriteInMainArea("Start zone encounter : 7");

            if (int.TryParse(Console.ReadLine(), out int input) == false || input > 7 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, 4, 5, 6 or 7");
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
            else if (input == 5) StartTestCombat(new List<Enemy> { CloneEnemy(EnemyLibrary.Thug) });
            else if (input == 6) StartTestCombat(new List<Enemy> { CloneEnemy(EnemyLibrary.Thug), CloneEnemy(EnemyLibrary.VampireSpawn) });
            else if (input == 7) StartZoneEncounter(LocationLibrary.starterTown, LocationLibrary.mountain, 3);

            db.SavePlayer(player);
        }

        public static void ShowPlayerStats()
        {
            MainUI.ClearMainArea();

            MainUI.WriteInMainArea($"\nAccount Name: {player.name} \n\nLevel: {player.level} \nClass: {player.playerClass} \nHP: {player.HP}/{player.maxHP} \nDMG: {player.DMG} \nSpeed: {player.speed} \narmor: {player.armor}" +
                $"\nDodge: {player.dodge} \nDodgeNegation: {player.dodgeNegation} \nCrit-chance: {player.critChance} \nCrit-Damage: {player.critDamage} \nStun: {player.stun}" +
                $"\nStunNegation: {player.stunNegation}\n\n");

            Thread.Sleep(1000);
            MainUI.WriteInMainArea("-press Enter to continue");

            Console.ReadLine();

            MainMenu();
        }

        public static void SavePlayer()
        {
            db.SavePlayer(player);
        }

        private static Enemy CloneEnemy(Enemy e)
        {
            var clone = new Enemy(e.name, e.level, e.exp, e.HP, e.DMG, e.speed, e.armor, e.dodge, e.dodgeNegation, e.critChance, e.critDamage, e.stun, e.stunNegation, e.money)
            {
                maxHP = e.maxHP > 0 ? e.maxHP : e.HP,
                attacks = e.attacks?.ToList() ?? new List<Attack>()
            };
            return clone;
        }

        private static void StartTestCombat(List<Enemy> testEnemies)
        {
            var cm = new CombatManager(player, testEnemies);
            cm.StartCombat();
            MainUI.WriteInMainArea("\nReturning to main menu...\n");
            MainMenu();
        }

        private static void StartZoneEncounter(Location a, Location b, int count)
        {
            var combined = new Dictionary<Enemy, int>();
            void addAll(Dictionary<Enemy,int> src)
            {
                if (src == null) return;
                foreach (var kv in src)
                {
                    if (combined.ContainsKey(kv.Key)) combined[kv.Key] += kv.Value;
                    else combined[kv.Key] = kv.Value;
                }
            }
            addAll(a.possibleEnemy);
            addAll(b.possibleEnemy);

            if (combined.Count == 0)
            {
                MainUI.WriteInMainArea("No enemies in these zones.");
                MainMenu();
                return;
            }

            int total = combined.Values.Sum();
            var picks = new List<Enemy>();
            for (int i = 0; i < count; i++)
            {
                int r = rng.Next(0, total);
                int accum = 0;
                foreach (var kv in combined)
                {
                    accum += kv.Value;
                    if (r < accum)
                    {
                        picks.Add(CloneEnemy(kv.Key));
                        break;
                    }
                }
            }

            var cm = new CombatManager(player, picks);
            cm.StartCombat();
            MainUI.WriteInMainArea("\nReturning to main menu...\n");
            MainMenu();
        }
    }
}

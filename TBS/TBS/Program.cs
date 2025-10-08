using System.Drawing;

namespace Game.Class
{
    public class Program
    {
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static Inventory Inventory;
        static Settings settings;
        static AttackManager atkManager;
        static Random rng = new Random();
        public static void Main(string[] args)
        {
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
            settings = new Settings();
            settings.ChangeTextColor();
            MainMenu();
        }
        public static void MainMenu()
        {
            Console.WriteLine($"\nwhat do you wish to do? (type the number next to it) \nGo somewhere : 0 \nCheck Inventory : 1 \n" +
                $"Check Moves : 2\ndo something here current location {player.currentLocation.name} : 3 \n" +
                $"change settings : 4 \n" +
                $"Start test combat (1v1) : 5\nStart test combat (1v2) : 6\n" +
                $"Start zone encounter StarterTown <-> Mountain (3 enemies) : 7\n");
            //int.TryParse(Console.ReadLine(), out int input);
            if (int.TryParse(Console.ReadLine(), out int input) == false || input > 6 || input < 0)
            {
                Console.WriteLine("\nyou gotta type 0, 1, 2 or 3");
                MainMenu();
                return;
            }
            else if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1) Inventory.ShowInventory();
            else if (input == 2) atkManager.ShowMovesMenu();
            else if (input == 3)
            {
                Console.WriteLine("all establisments in your current location");
                int i = 0;
                foreach (var subLocation in player.currentLocation.subLocationsHere)
                {
                    i++;
                    Console.WriteLine($"{subLocation.name} : {i}");
                }
                if (i == 0)
                {
                    Console.Clear();
                    Console.WriteLine("there arent any establisments in your current location sorry");
                    MainMenu();
                    return;
                }

                Console.WriteLine("\ntype out the number next to the location you want to go to\n or leave : 0");

                int targetDes;
                if (int.TryParse(Console.ReadLine(), out targetDes))
                {
                    if (targetDes == 0)
                    {
                        Console.Clear();
                        MainMenu();
                        return;
                    }
                    else if (targetDes > player.currentLocation.subLocationsHere.Count || targetDes < 0)
                    {
                        Console.WriteLine("that number is wrong mate");
                        MainMenu();
                        return;
                    }
                    player.currentLocation.subLocationsHere[targetDes - 1].DoSubLocation();
                }
                else
                {
                    Console.WriteLine("write a number dumb dumb");
                    MainMenu();
                    return;
                }

            }
            else if (input == 4) settings.ChangeTextColor();
            else if (input == 5) StartTestCombat(new List<Enemy> { CloneEnemy(EnemyLibrary.Thug), CloneEnemy(EnemyLibrary.Goblin) });
            else if (input == 6) StartTestCombat(new List<Enemy> { CloneEnemy(EnemyLibrary.Thug), CloneEnemy(EnemyLibrary.VampireSpawn) });
            else if (input == 7) StartZoneEncounter(LocationLibrary.starterTown, LocationLibrary.mountain, 3);



            db.SavePlayer(player);
        }



        public static void SavePlayer()
        {
            db.SavePlayer(player);
        }

        // Temp under bare for at teste shit, ikke permenent

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
            Console.WriteLine("\nReturning to main menu...\n");
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
            addAll(a.PossibleEncounters);
            addAll(b.PossibleEncounters);

            if (combined.Count == 0)
            {
                Console.WriteLine("No enemies in these zones.");
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
            Console.WriteLine("\nReturning to main menu...\n");
            MainMenu();
        }

    }
}

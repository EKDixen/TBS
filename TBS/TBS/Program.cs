namespace Game.Class
{
    public class Program
    {
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static Inventory Inventory = new Inventory();
        static AttackManager atkManager;
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
            journeyManager.AddLocations();
            MainMenu();
        }
        public static void MainMenu()
        {
            Console.WriteLine($"\nwhat do you wish to do? (type the number next to it) \nGo somewhere : 0 \nCheck Inventory : 1 \n" +
                $"Check Moves : 2\ndo something here current location {player.currentLocation.name} : 3 \n");
            int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input > 3)
            {
                Console.WriteLine("\n");
                MainMenu();
                return;
            }
            else if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1) Inventory.ShowInventory();
            else if (input == 2) atkManager.ShowMovesMenu();
            else if (input == 3) { Console.WriteLine("\nthis hasnt been added yet\n"); MainMenu(); }
            db.SavePlayer(player);
        }



        public static void SavePlayer()
        {
            db.SavePlayer(player);
        }

    }
}

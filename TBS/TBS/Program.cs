namespace Game.Class
{
    public class Program
    {
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static Inventory Inventory = new Inventory();
        public static void Main(string[] args)
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
                    Console.WriteLine($"Welcome back, {player.playerName} (Level {player.level})!");
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                    return;
                }
            }
            else if (choice == "2")
            {
                PlayerCreator creator = new PlayerCreator();
                player = creator.PlayerCreatorFunction(db);
                db.SavePlayer(player);
                Console.WriteLine("New character created and saved!");
            }
            else
            {
                Console.WriteLine("Invalid choice, exiting.");
                return;
            }


            journeyManager.AddLocations();
            MainMenu();
        }
        public static void MainMenu()
        {
            Console.WriteLine($"what do you wish to do? (type the number next to it) \nGo somewhere : 0 \nCheck Inventory : 1 \n" +
                $"do something here current location {player.currentLocation.name} : 2 \n");
            int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input > 2)
            {
                Console.WriteLine("\n");
                MainMenu();
                return;
            }
            else if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1) Inventory.ShowInventory();
            else if (input == 2) { Console.WriteLine("this hasnt been added yet"); MainMenu();}
        }



        public static void SavePlayer()
        {
            db.SavePlayer(player);
        }

    }
}

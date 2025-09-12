namespace Game.Class
{
    public class Program
    {
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
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
                player = creator.PlayerCreatorFunction();
                db.SavePlayer(player);
                Console.WriteLine("New character created and saved!");
            }
            else
            {
                Console.WriteLine("Invalid choice, exiting.");
                return;
            }

            JourneyManager journeyManager = new JourneyManager();
            journeyManager.AddLocations();
            journeyManager.ChoseTravelDestination();
        }
        public static void SavePlayer()
        {
            db.SavePlayer(player);
        }

    }
}

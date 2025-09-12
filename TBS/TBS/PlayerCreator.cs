public class PlayerCreator
{
    public List<Player> players = new List<Player>();
    public Player PlayerCreatorFunction()
    {
        Console.WriteLine("Name your character (needed to login)");
        string name = Console.ReadLine();
        Console.WriteLine("Write a password for you character (needed to login)");
        string password = Console.ReadLine();
        Player newPlayer = new Player(name, password, "Pathfinder", 1, 0, 100, 10, 10, 0, 105, 100, 105, 200, 105, 100, null, 10, 100);
        players.Add(newPlayer);
        return newPlayer;
    }
}

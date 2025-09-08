public class Program
{
    public static void Main(string[] args)
    {
        // create a player using your PlayerCreator
        PlayerCreator creator = new PlayerCreator();
        Player player = creator.PlayerCreatorFunction();

        Console.WriteLine($"Player created: {player.playerName}, Class: {player.playerClass}");
    }
}

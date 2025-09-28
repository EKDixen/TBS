public class PlayerCreator
{
    public List<Player> players = new List<Player>();
    public Player PlayerCreatorFunction(PlayerDatabase db)
    {
        Console.WriteLine("Name your character (needed to login)");
        string name = Console.ReadLine();
        
        if (db.PlayerExists(name))
        {
            Console.WriteLine("A player with that name already exists, please choose another name");
            return PlayerCreatorFunction(db);
        }
        Console.WriteLine("Write a password for you character (needed to login)");
        string password = Console.ReadLine();
        Player newPlayer = new Player(name, password, "Pathfinder", 1, 0, 100, 10, 10, 0, 105, 100, 105, 200, 105, 100, null, 10, 100);
        AttackManager atkManager = new AttackManager(newPlayer);
        atkManager.LearnAttack(AttackLibrary.ThrowHands);
        atkManager.EquipAttack(AttackLibrary.ThrowHands, 1);
        Inventory inv = new Inventory(newPlayer);
        Item rock = new Item("Rock","Useless",1, 3, ItemType.equipment);
        inv.AddItem(rock);
        players.Add(newPlayer);
        return newPlayer;
    }
}
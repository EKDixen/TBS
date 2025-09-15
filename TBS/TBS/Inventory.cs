using Game.Class;
public class Inventory
{

    public void ShowInventory()
    {
        Console.WriteLine("\n     Name            Qty   Description");
        Console.WriteLine("--------------------------------------");
        int i = 0;
        foreach (var item in Program.player.ownedItems)
        {
            i++;
            Console.WriteLine($"{i,-5}{item.name,-15} {item.amount,-5} {item.description}");
        }
        Console.WriteLine("\n \n ");
        Program.MainMenu();
    }

}

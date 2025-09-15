using Game.Class;
public class Inventory
{
    Item item = new Item("name","description",1,ItemType.equipment);
    Item item2 = new Item("name2", "description2", 16,ItemType.equipment);
    public void AddItems()
    {
        Program.player.ownedItems.Add(item);
        Program.player.ownedItems.Add(item2);
    }
    public void ShowInventory()
    {
        AddItems();

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

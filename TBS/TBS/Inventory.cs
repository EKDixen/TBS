using Game.Class;
public class Inventory
{
    /*Item item = new Item("name","description",1,ItemType.equipment);
    Item item2 = new Item("name2", "description2", 16,ItemType.equipment);
    item.Stats["DMG"] = 10;                                                                        ------this is just for test
    public void AddItems()
    {
        Program.player.ownedItems.Add(item);
        Program.player.ownedItems.Add(item2);
    } */
    public void ShowInventory()
    {
        Console.WriteLine("\n     Name            Qty   Description");
        Console.WriteLine("--------------------------------------");
        int i = 1;
        foreach (var item in Program.player.ownedItems)
        {
            i++;
            Console.WriteLine($"{i,-5}{item.name,-15} {item.amount,-5} {item.description}");
        }
        Console.WriteLine("if you want to interact with anything type its corresponding number \nif not type 0");
        var n = int.TryParse(Console.ReadLine(), out int input);
        if (input == null)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number\n ");
            ShowInventory();
            return;
        }
        else if (input == 0) { Program.MainMenu(); return; }

        Console.WriteLine($"you've picked{Program.player.ownedItems[input].name}");


        Console.WriteLine("0 : details");
        Console.WriteLine("1 : drop");
        if (Program.player.ownedItems[input].type == ItemType.consumable)  Console.WriteLine("2 : consume");
        Console.WriteLine("\ntype out the number next to the action you want to perform");
        var k = int.TryParse(Console.ReadLine(), out int ik);
        if (ik == null)
        {
            Console.Clear();
            Console.WriteLine("my love would you please type a number this time\n ");
            ShowInventory();
            return;
        }
        else if (ik == 0)
        {
            Console.WriteLine("lwk dont know what to put here:3");
            //Console.WriteLine($"the name is {Program.player.ownedItems[input].name} and its ");
        }
        else if (ik ==1) 
        {
            Console.WriteLine($"\nyou drop the {Program.player.ownedItems[input].name}");
            Program.player.ownedItems.Remove(Program.player.ownedItems[input]);
        }
        else if (ik == 2)
        {
            Console.WriteLine("this functionally hasnt been added yet");
        }
        Program.SavePlayer();
        Program.MainMenu();
    }
    

}

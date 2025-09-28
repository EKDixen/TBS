using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Game.Class;

public enum SubLocationType
{
    Shop,
    Bar,
    Hotel
}
public class SubLocation
{
    public SubLocationType type;
    public string name;
    public List<Item> shopItems = new List<Item>();

    public SubLocation(string tName,SubLocationType tType)
    {
        name = tName;
        type = tType;
    }


    
    public void DoSubLocation()
    {
        if (type == SubLocationType.Shop)
        {
            Inventory inventory = new Inventory(Program.player);



            Console.WriteLine("\n nr     Name            Qty   Description    Price");
            Console.WriteLine(" --------------------------------------------------");
            int i = 0;
            foreach (var item in shopItems)
            {
                i++;
                Console.WriteLine($" {i,-7}{item.name,-15} {item.amount,-5} {item.description, -16} {item.value}");
            }
            Console.WriteLine("\nif you want to interact with anything type its corresponding number \nif not type 0");
            var n = int.TryParse(Console.ReadLine(), out int input);
            if (input == null)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0) { Program.MainMenu(); return; }
            input--;
            Console.WriteLine($"you've picked {shopItems[input].name}");


            Console.WriteLine("0 : details");
            Console.WriteLine("1 : buy");
            Console.WriteLine("\ntype out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadLine(), out int ik);
            if (ik == null)
            {
                Console.Clear();
                Console.WriteLine("my love would you please type a number this time\n ");
                DoSubLocation();
                return;
            }
            else if (ik == 0)
            {
                Console.Clear();
                Console.WriteLine("lwk dont know what to put here sryy:3");
            }
            else if (ik == 1)
            {
                inventory.AddItem(shopItems[input]);
                //add payment
            }
            Program.SavePlayer();
            Program.MainMenu();
        }


    }



}


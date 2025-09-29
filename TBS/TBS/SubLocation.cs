using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Game.Class;

public enum SubLocationType
{
    shop,
    tavern,
    resturant,
    blacksmith,
    arena,
    bank



}
public class SubLocation
{
    public SubLocationType type;
    public string name;
    public List<Item> shopItems = new List<Item>();
    public List<Item> bankItems = new List<Item>();
    public int bankMoney = 0;
    public SubLocation() { } //Deserialize

    public SubLocation(string tName,SubLocationType tType)
    {
        name = tName;
        type = tType;
    }


    
    public void DoSubLocation()
    {
        if (type == SubLocationType.shop)
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
            if (input == null || input > shopItems.Count || input < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0) { Program.MainMenu(); return; }
            input--;
            Console.WriteLine($"\nyou've picked {shopItems[input].name}\nit costs {shopItems[input].value}\nyou have {Program.player.money} money");

            Console.WriteLine("0 : details");
            Console.WriteLine("1 : buy");
            Console.WriteLine("\ntype out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadLine(), out int ik);
            if (ik == null || ik < 0 || ik > 1)
            {
                Console.Clear();
                Console.WriteLine("my love would you please type a functional number this time\n ");
                DoSubLocation();
                return;
            }
            else if (ik == 0)
            {
                Console.Clear();
                Console.WriteLine($"\nyou've picked {shopItems[input].name}");
                Console.WriteLine($"{shopItems[input].details}\n");
            }
            else if (ik == 1)
            {
                if ((Program.player.money - shopItems[input].value) >= 0)
                {
                    inventory.AddItem(shopItems[input]);
                    Program.player.money -= shopItems[input].value;
                }
                else 
                { 
                    Console.Clear();
                    Console.WriteLine("you dont have enough money for that");
                    Program.MainMenu();
                    return;
                }

            }
            Program.SavePlayer();
            Program.MainMenu();
        }
        if (type == SubLocationType.bank)
        {



            Console.WriteLine($"you have {bankMoney} money stored here and heres all the items you have stored here:");
            Console.WriteLine("\n nr     Name            Qty   Description    value");
            Console.WriteLine(" --------------------------------------------------");
            int i = 0;
            foreach (var item in bankItems)
            {
                i++;
                Console.WriteLine($" {i,-7}{item.name,-15} {item.amount,-5} {item.description,-16} {item.value}");
            }
            Console.WriteLine("if you want to grab something type its nr \nif you want to deposit something or leave then type 0");
            var n = int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input > bankItems.Count || input < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0) 
            {
                Console.WriteLine("do you want to leave type 0, if you want to diposit type 1");
                var n2 = int.TryParse(Console.ReadLine(), out int input2);
                if (input2 == null || input2 > 1 || input2 < 0)
                {
                    Console.Clear();
                    Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                    DoSubLocation();
                    return;
                }
                else if (input2 == 0) { Program.MainMenu(); return; }
                else 
                {
                    Console.WriteLine($"you have {Program.player.money} money\n\nand these are your items");

                    Console.WriteLine("\nnr     Name            Qty   Description     value");
                    Console.WriteLine("--------------------------------------------------");
                    int id = 0;
                    foreach (var item in Program.player.ownedItems)
                    {
                        id++;
                        Console.WriteLine($"{id,-7}{item.name,-15} {item.amount,-5} {item.description,-16} {item.value}");
                    }


                }
            }
        }

    }



}


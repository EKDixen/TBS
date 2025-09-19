using Game.Class;
public class Inventory
{
    // THIS IS HOW YOU MAKE ITEM DUMB DUMB <3
    //
    //
    //        item = new Item("name2", "description2", 16, ItemType.equipment);
    //        item.stats["DMG"] = 10;

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
            RemoveEffects(Program.player.ownedItems[input]);
            Program.player.ownedItems.Remove(Program.player.ownedItems[input]);
        }
        else if (ik == 2)
        {
            Console.WriteLine("this functionally hasnt been added yet");
        }
        Program.SavePlayer();
        Program.MainMenu();
    }
    public void AddItem(Item Titem) 
    {
        Program.player.ownedItems.Add(Titem);
        ApplyEffects(Titem);
    }
    public void ApplyEffects(Item Titem)
    {
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP":Program.player.HP += Titem.stats["HP"];break;
                case "DMG":Program.player.DMG += Titem.stats["DMG"];break;
                case "speed":Program.player.speed += Titem.stats["speed"];break;
                case "armor":Program.player.armor += Titem.stats["armor"];break;
                case "dodge":Program.player.dodge += Titem.stats["dodge"];break;
                case "dodgeNegation":Program.player.dodgeNegation += Titem.stats["dodgeNegation"];break;
                case "critChance":Program.player.critChance += Titem.stats["critChance"];break;
                case "critDamage":Program.player.critDamage += Titem.stats["critDamage"]; break;
                case "stun":Program.player.stun += Titem.stats["stun"];break;
                case "stunNegation":Program.player.stunNegation += Titem.stats["stunNegation"];break;

            }
        }
    }
    public void RemoveEffects(Item Titem)
    {
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP": Program.player.HP -= Titem.stats["HP"]; break;
                case "DMG": Program.player.DMG -= Titem.stats["DMG"]; break;
                case "speed": Program.player.speed -= Titem.stats["speed"]; break;
                case "armor": Program.player.armor -= Titem.stats["armor"]; break;
                case "dodge": Program.player.dodge -= Titem.stats["dodge"]; break;
                case "dodgeNegation": Program.player.dodgeNegation -= Titem.stats["dodgeNegation"]; break;
                case "critChance": Program.player.critChance -= Titem.stats["critChance"]; break;
                case "critDamage": Program.player.critDamage -= Titem.stats["critDamage"]; break;
                case "stun": Program.player.stun -= Titem.stats["stun"]; break;
                case "stunNegation": Program.player.stunNegation -= Titem.stats["stunNegation"]; break;

            }
        }
    }
}

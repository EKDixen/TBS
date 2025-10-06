using Game.Class;
public class Inventory
{
    // THIS IS HOW YOU MAKE ITEM DUMB DUMB <3
    //
    //
    //        item = new Item("name2", "description2", 16,2 , ItemType.equipment);
    //        item.stats["DMG"] = 10;
    private Player player;

    public Inventory(Player p)
    {
        player = p;
    }
    public void ShowInventory()
    {
        Console.Clear();
        Console.WriteLine($"you have {player.money} money\n\nand these are your items");

        Console.WriteLine("\nnr     Name            Qty   Description     value");
        Console.WriteLine("--------------------------------------------------");
        int i = 0;
        foreach (var item in player.ownedItems)
        {
            i++;
            Console.WriteLine($"{i,-7}{item.name,-15} {item.amount,-5} {item.description,-16} {item.value}");
        }
        Console.WriteLine("\nif you want to interact with anything type its corresponding number \nif not type 0");
        var n = int.TryParse(Console.ReadLine(), out int input);
        if (input == null || input < 0 || input > player.ownedItems.Count)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a usable number\n ");
            ShowInventory();
            return;
        }
        else if (input == 0) { Program.MainMenu(); return; }

        input--;
        Console.WriteLine($"you've picked {player.ownedItems[input].name}");


        Console.WriteLine("0 : details");
        Console.WriteLine("1 : drop");
        if (player.ownedItems[input].type == ItemType.consumable)  Console.WriteLine("2 : consume");
        Console.WriteLine("\ntype out the number next to the action you want to perform");
        var k = int.TryParse(Console.ReadLine(), out int ik);
        if (ik == null || ik <0 || ik > 2)
        {
            Console.Clear();
            Console.WriteLine("my love would you please type a number this time\n ");
            ShowInventory();
            return;
        }
        else if (ik == 0)
        {
            Console.WriteLine($"\nyou've picked {player.ownedItems[input].name}");
            Console.WriteLine($"{player.ownedItems[input].details}\n");
        }
        else if (ik ==1) 
        {
            Console.WriteLine($"\nyou drop the {player.ownedItems[input].name}");
            DropItem(player.ownedItems[input]);
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
        if (player.ownedItems.Contains(Titem))
        {
            Item existingItem = player.ownedItems.First(i => i.Equals(Titem));
            existingItem.amount += Titem.amount;
            if (Titem.type != ItemType.consumable) ApplyEffects(Titem);
        }
        else
        {
            player.ownedItems.Add(Titem);
            if (Titem.type != ItemType.consumable) ApplyEffects(Titem);
        }

    }
    public void DropItem(Item Titem)
    {
        if (Titem.type != ItemType.consumable) RemoveEffects(Titem);
        player.ownedItems.Remove(Titem);
    }
    public void ApplyEffects(Item Titem)
    {
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP":player.HP += Titem.stats["HP"]*Titem.amount;break;
                case "DMG":player.DMG += Titem.stats["DMG"] * Titem.amount; break;
                case "speed":player.speed += Titem.stats["speed"] * Titem.amount; break;
                case "armor":player.armor += Titem.stats["armor"] * Titem.amount; break;
                case "dodge":player.dodge += Titem.stats["dodge"] * Titem.amount; break;
                case "dodgeNegation":player.dodgeNegation += Titem.stats["dodgeNegation"] * Titem.amount;break;
                case "critChance":player.critChance += Titem.stats["critChance"] * Titem.amount; break;
                case "critDamage":player.critDamage += Titem.stats["critDamage"] * Titem.amount; break;
                case "stun":player.stun += Titem.stats["stun"] * Titem.amount; break;
                case "stunNegation":player.stunNegation += Titem.stats["stunNegation"] * Titem.amount; break;

            }
        }
    }
    public void RemoveEffects(Item Titem)
    {
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP": player.HP -= Titem.stats["HP"]; break;
                case "DMG": player.DMG -= Titem.stats["DMG"]; break;
                case "speed": player.speed -= Titem.stats["speed"]; break;
                case "armor": player.armor -= Titem.stats["armor"]; break;
                case "dodge": player.dodge -= Titem.stats["dodge"]; break;
                case "dodgeNegation": player.dodgeNegation -= Titem.stats["dodgeNegation"]; break;
                case "critChance": player.critChance -= Titem.stats["critChance"]; break;
                case "critDamage": player.critDamage -= Titem.stats["critDamage"]; break;
                case "stun": player.stun -= Titem.stats["stun"]; break;
                case "stunNegation": player.stunNegation -= Titem.stats["stunNegation"]; break;

            }
        }
    }
}

using Game.Class;
public class Inventory
{
    private Player player;

    public Inventory(Player p)
    {
        player = p;

        while (player.equippedItems.Count < 4)
        {
            player.equippedItems.Add(null);
        }
    }
    public void ShowInventory()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("Equiped items:");
            for (int j = 0; j < player.equippedItems.Capacity; j++)
            {
                string place = "";
                switch (j)
                {
                    case 0: place = "Head"; break;
                    case 1: place = "Torso";break;
                    case 2: place = "Legs"; break;
                    case 3: place = "Feet"; break;
                }
                Console.WriteLine($"{j + 1} ({place}) : {player.equippedItems[j]?.name ?? "Empty"}");


            }

            Console.WriteLine($"\nyou have {player.money} money\n\nand these are your items");

            Console.WriteLine("\nnr     Name                      Qty   Description         value");
            Console.WriteLine("----------------------------------------------------------------");
            int i = 0;
            foreach (var item in player.ownedItems)
            {
                i++;

                // check if equipped
                int slotIndex = player.equippedItems.IndexOf(item);
                string equippedInfo = slotIndex >= 0 ? $"(Slot {slotIndex + 1})" : "";


                Console.WriteLine($"{i,-7}{item.name,-25} {item.amount,-5} {item.description,-20} {item.value} {equippedInfo}");
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
            if (player.ownedItems[input].type == ItemType.equipment) Console.WriteLine("2 : Equip/Unequip");
            if (player.ownedItems[input].type == ItemType.consumable) Console.WriteLine("2 : consume");
            Console.WriteLine("\ntype out the number next to the action you want to perform");
            var k = int.TryParse(Console.ReadLine(), out int ik);
            if (ik == null || ik < 0 || ik > 2)
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
            else if (ik == 1)
            {
                Console.WriteLine($"\nyou drop the {player.ownedItems[input].name}");
                DropItem(player.ownedItems[input]);
            }
            else if (ik == 2 && player.ownedItems[input].type == ItemType.equipment)
            {

                Item chosen = player.ownedItems[input];

                // if already equipped, unequip
                int equippedSlot = player.equippedItems.IndexOf(chosen);
                if (equippedSlot >= 0)
                {
                    Console.WriteLine($"{chosen.name} is currently in Slot {equippedSlot + 1}. Unequipping...");
                    RemoveEffects(chosen);
                    player.equippedItems[equippedSlot] = null;
                }
                else
                {
                    // equip
                    Console.Write("Choose a slot (1-4) or 0 to cancel: ");
                    if (!int.TryParse(Console.ReadLine(), out int slot) || slot < 0 || slot > 4) continue;
                    if (slot == 0) continue;
                    if (slot == 1 && chosen.equipmentType != EquipmentType.head) continue;
                    if (slot == 2 && chosen.equipmentType != EquipmentType.torso) continue;
                    if (slot == 3 && chosen.equipmentType != EquipmentType.legs) continue;
                    if (slot == 4 && chosen.equipmentType != EquipmentType.feet) continue;

                    ApplyEffects(chosen);
                    player.equippedItems[slot - 1] = chosen;
                    Console.WriteLine($"{chosen.name} equipped into Slot {slot}!");

                }



            }
            else if (ik == 2 && player.ownedItems[input].type == ItemType.consumable)
            {
                Consume(player.ownedItems[input]);
            }
            Program.SavePlayer();
            Program.MainMenu();
            break;
        }
    }
    public void AddItem(Item Titem) 
    {
        if (player.ownedItems.Contains(Titem))
        {
            Item existingItem = player.ownedItems.First(i => i.Equals(Titem));
            existingItem.amount += Titem.amount;
            if (Titem.type == ItemType.Artifact) ApplyEffects(Titem);
        }
        else
        {
            player.ownedItems.Add(Titem);
            if (Titem.type == ItemType.Artifact) ApplyEffects(Titem);
        }

    }
    public void DropItem(Item Titem)
    {
        if (Titem.type == ItemType.Artifact) RemoveEffects(Titem);
        int equippedSlot = player.equippedItems.IndexOf(Titem);
        if(equippedSlot >= 0)
        {
            UnequipItem(equippedSlot);
            RemoveEffects(Titem);
        }
        player.ownedItems.Remove(Titem);
    }
    public void ApplyEffects(Item Titem)
    {
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP":if((player.HP += Titem.stats["HP"] * Titem.amount) > player.maxHP) player.HP = player.maxHP; else player.HP += Titem.stats["HP"]*Titem.amount;break;
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
                case "HP": player.HP -= Titem.stats["HP"] * Titem.amount; break;
                case "DMG": player.DMG -= Titem.stats["DMG"] * Titem.amount; break;
                case "speed": player.speed -= Titem.stats["speed"] * Titem.amount; break;
                case "armor": player.armor -= Titem.stats["armor"] * Titem.amount; break;
                case "dodge": player.dodge -= Titem.stats["dodge"] * Titem.amount; break;
                case "dodgeNegation": player.dodgeNegation -= Titem.stats["dodgeNegation"] * Titem.amount; break;
                case "critChance": player.critChance -= Titem.stats["critChance"] * Titem.amount; break;
                case "critDamage": player.critDamage -= Titem.stats["critDamage"] * Titem.amount; break;
                case "stun": player.stun -= Titem.stats["stun"] * Titem.amount; break;
                case "stunNegation": player.stunNegation -= Titem.stats["stunNegation"] * Titem.amount; break;

            }
        }
    }
    public void Consume(Item Titem)
    {
        if (Titem.duration == 0)
        {
            ApplyEffects(Titem);
            DropItem(Titem);
        }
        else
        {
            Console.WriteLine("not done");
        }
    }

    public void UnequipItem(int slot)
    {
        if (player.equippedItems[slot] != null)
        {
            Console.WriteLine($"{player.equippedItems[slot].name} unequipped from Slot {slot}.");
            player.equippedItems[slot] = null;
        }
    }
}

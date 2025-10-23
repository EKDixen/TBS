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
            MainUI.ClearMainArea();

            MainUI.WriteInMainArea("Equiped items:");
            for (int j = 0; j < player.equippedItems.Capacity; j++)
            {
                string place = "";
                switch (j)
                {
                    case 0: place = "Head"; break;
                    case 1: place = "Body";break;
                    case 2: place = "Legs"; break;
                    case 3: place = "Feet"; break;
                }
                MainUI.WriteInMainArea($"{j + 1} ({place}) : {player.equippedItems[j]?.name ?? "Empty"}");


            }

            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"you have {player.money} money\n\nand these are your items");

            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty   Description         value");
            MainUI.WriteInMainArea("----------------------------------------------------------------");
            int i = 0;
            foreach (var item in player.ownedItems)
            {
                i++;

                // check if equipped
                int slotIndex = player.equippedItems.IndexOf(item);
                string equippedInfo = slotIndex >= 0 ? $"(Slot {slotIndex + 1})" : "";


                MainUI.WriteInMainArea($"{i,-7}{item.name,-25} {item.amount,-5} {item.description,-20} {item.value} {equippedInfo}");
            }
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("if you want to interact with anything type its corresponding number");
            MainUI.WriteInMainArea("if not type 0");

            var n = int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input < 0 || input > player.ownedItems.Count)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a usable number ");
                MainUI.WriteInMainArea("");

                ShowInventory();
                return;
            }
            else if (input == 0) { Program.MainMenu(); return; }

            input--;
            MainUI.WriteInMainArea($"you've picked {player.ownedItems[input].name}");


            MainUI.WriteInMainArea("0 : details");
            MainUI.WriteInMainArea("1 : drop");
            if (player.ownedItems[input].type == ItemType.equipment) MainUI.WriteInMainArea("2 : Equip/Unequip");
            if (player.ownedItems[input].type == ItemType.consumable) MainUI.WriteInMainArea("2 : consume");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("type out the number next to the action you want to perform");
            var k = int.TryParse(Console.ReadLine(), out int ik);
            if (ik == null || ik < 0 || ik > 2)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("my love would you please type a number this time\n ");
                ShowInventory();
                return;
            }
            else if (ik == 0)
            {
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea($"you've picked {player.ownedItems[input].name}");
                MainUI.WriteInMainArea($"{player.ownedItems[input].details}\n");

                MainUI.WriteInMainArea($"-press Enter to continue");
                Console.ReadLine();
            }
            else if (ik == 1)
            {
                MainUI.WriteInMainArea($"\nyou drop the {player.ownedItems[input].name}");
                DropItem(player.ownedItems[input]);
            }
            else if (ik == 2 && player.ownedItems[input].type == ItemType.equipment)
            {

                Item chosen = player.ownedItems[input];

                // if already equipped, unequip
                int equippedSlot = player.equippedItems.IndexOf(chosen);
                if (equippedSlot >= 0)
                {
                    MainUI.WriteInMainArea($"{chosen.name} is currently in Slot {equippedSlot + 1}. Unequipping...");
                    RemoveEffects(chosen);
                    player.equippedItems[equippedSlot] = null;
                }
                else
                {
                    // equip
                    int slot = 0;
                    if (chosen.equipmentType == EquipmentType.head) slot = 1;
                    if (chosen.equipmentType == EquipmentType.torso) slot = 2;
                    if (chosen.equipmentType == EquipmentType.legs) slot = 3;
                    if (chosen.equipmentType == EquipmentType.feet) slot = 4;

                    ApplyEffects(chosen);
                    player.equippedItems[slot - 1] = chosen;
                    MainUI.WriteInMainArea($"{chosen.name} equipped into Slot {slot}!");

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
    public void AddItem(Item Titem, int tAmount) 
    {
        if (player.ownedItems.Contains(Titem) && Titem.type != ItemType.equipment)
        {
            Item existingItem = player.ownedItems.First(i => i.Equals(Titem));
            existingItem.amount += tAmount;
            if (Titem.type == ItemType.Artifact) ApplyEffects(Titem);
        }
        else
        {
            Titem.amount = tAmount;
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
        if (Titem.amount <= 1) player.ownedItems.Remove(Titem);
        else Titem.amount--;
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
            MainUI.WriteInMainArea("not done");
        }
    }

    public void UnequipItem(int slot)
    {
        if (player.equippedItems[slot] != null)
        {
            MainUI.WriteInMainArea($"{player.equippedItems[slot].name} unequipped from Slot {slot}.");
            player.equippedItems[slot] = null;
        }
    }
}

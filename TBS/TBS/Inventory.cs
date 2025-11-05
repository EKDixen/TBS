using Game.Class;
using System.Reflection.Metadata;
using static System.Formats.Asn1.AsnWriter;
public class Inventory
{
    private Player player;

    private int currentPage = 1; 
    private int itemsPerPage = 8;  
    private string searchTerm = ""; 
    private List<Item> filteredItems; // This will hold the items we are currently viewing

    const float exponent = 1.5f;
    const float scale = 0.1f;

    public Inventory(Player p)
    {
        player = p;

        while (player.equippedItems.Count < 4)
        {
            player.equippedItems.Add(null);
        }

        filteredItems = player.ownedItems;
    }
    public void ShowInventory()
    {
        while (true)
        {
            // Update the filtered list based on the search term
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredItems = player.ownedItems; // full list
            }
            else
            {
                filteredItems = player.ownedItems
                    .Where(item => item.name.ToLower().Contains(searchTerm.ToLower()) ||
                                   item.description.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }
             
            // Calculate total pages and get the items for the current page
            int totalItems = filteredItems.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            if (totalPages == 0) totalPages = 1; 
            if (currentPage > totalPages) currentPage = totalPages; // Fix if we are on a page that no longer exists
            if (currentPage < 1) currentPage = 1;

            List<Item> pageItems = filteredItems
                .Skip((currentPage - 1) * itemsPerPage) // Skip items on previous pages
                .Take(itemsPerPage)                     // Get just the items for this page
                .ToList();


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
            MainUI.WriteInMainArea($"you have {player.money} Rai\n");
            MainUI.WriteInMainArea($"your items weight {player.inventoryWeight} therefore your speed is being reduced by {(int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent))} \n");

            if (!string.IsNullOrEmpty(searchTerm))
            {
                MainUI.WriteInMainArea($"\nShowing results for: \"{searchTerm}\"");
            }

            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty   Description         value");
            MainUI.WriteInMainArea("----------------------------------------------------------------");
            int i = 0;
            foreach (var item in pageItems)
            {
                i++;

                // check if equipped
                int slotIndex = player.equippedItems.IndexOf(item);
                string equippedInfo = slotIndex >= 0 ? $"(Slot {slotIndex + 1})" : "";


                MainUI.WriteInMainArea($"{i,-7}{item.name,-25} {item.amount,-5} {item.description,-20} {item.value} {equippedInfo}");
            }
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {currentPage} of {totalPages} ---");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Type item number (1-8) to interact, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back to Main Menu");


            string inputString = Console.ReadKey().KeyChar.ToString().ToLower() ?? "";

            if (inputString == "n")
            {
                if (currentPage < totalPages) currentPage++;
                continue; 
            }
            if (inputString == "p")
            {
                if (currentPage > 1) currentPage--;
                continue;
            }
            if (inputString == "s")
            {
                HandleSearch(); 
                continue;
            }

            var n = int.TryParse(inputString, out int input);

            if (input == 0) { Program.MainMenu(); return; } 

            if (!n || input < 1 || input > pageItems.Count)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a usable number *from this page* ");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("-press Enter to continue-");
                Console.ReadLine();
                continue; 
            }

            Item selectedItem = pageItems[input - 1];

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"you've picked {selectedItem.name}");

            MainUI.WriteInMainArea("0 : details");
            MainUI.WriteInMainArea("1 : drop");
            if (selectedItem.type == ItemType.equipment) MainUI.WriteInMainArea("2 : Equip/Unequip");
            if (selectedItem.type == ItemType.consumable) MainUI.WriteInMainArea("2 : consume");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("type out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int ik);
            if (!k || ik < 0 || ik > 2)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("my love would you please type a number this time\n ");

                MainUI.WriteInMainArea("-press Enter to continue-");
                Console.ReadLine();
                continue;
            }
            else if (ik == 0)
            {
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea($"you've picked {selectedItem.name}");
                MainUI.WriteInMainArea($"{selectedItem.details}\n");

                MainUI.WriteInMainArea($"-press Enter to continue");
                Console.ReadLine();
                continue;
            }
            else if (ik == 1)
            {
                MainUI.WriteInMainArea($"\nyou drop the {selectedItem.name}");
                DropItem(selectedItem);
            }
            else if (ik == 2 && selectedItem.type == ItemType.equipment)
            {

                Item chosen = selectedItem;

                // if already equipped, unequip
                int equippedSlot = player.equippedItems.IndexOf(chosen);
                if (equippedSlot >= 0)
                {
                    MainUI.WriteInMainArea($"{chosen.name} is currently in Slot {equippedSlot + 1}. Unequipping...");
                    UnequipItem(equippedSlot);
                }
                else
                {
                    // equip
                    int slot = 0;
                    if (chosen.equipmentType == EquipmentType.head) slot = 1;
                    if (chosen.equipmentType == EquipmentType.torso) slot = 2;
                    if (chosen.equipmentType == EquipmentType.legs) slot = 3;
                    if (chosen.equipmentType == EquipmentType.feet) slot = 4;

                    ApplyEffects(chosen, null);
                    player.equippedItems[slot - 1] = chosen;
                    MainUI.WriteInMainArea($"{chosen.name} equipped into Slot {slot}!");

                }



            }
            else if (ik == 2 && selectedItem.type == ItemType.consumable)
            {
                Consume(selectedItem);
            }
            Program.SavePlayer();
            Program.MainMenu();
            break;
        }
    }
    private void HandleSearch()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
        MainUI.WriteInMainArea("> ");
        searchTerm = Console.ReadLine()?.ToLower() ?? "";
        currentPage = 1; // ALWAYS reset to page 1 after a search
    }
    public void AddItem(Item templateItem, int tAmount)
    {
        Item existingItem = player.ownedItems.FirstOrDefault(i => i.name == templateItem.name);

        if (existingItem != null && templateItem.type != ItemType.equipment)
        {
            if (existingItem.type == ItemType.Artifact)
            {
                RemoveEffects(existingItem); 
            }

            player.inventoryWeight += existingItem.weight * tAmount;
            existingItem.amount += tAmount;

            if (existingItem.type == ItemType.Artifact)
            {
                ApplyEffects(existingItem, null); 
            }


                                        

            player.speed += (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));
            player.inventorySpeedModifier += existingItem.weight * tAmount;
            player.speed -= (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));
        }
        else
        {
            Item newItem = new Item(templateItem); // Use the copy constructor

            newItem.amount = tAmount;

            player.ownedItems.Add(newItem);
            player.inventoryWeight += newItem.weight * tAmount;

            if (newItem.type == ItemType.Artifact)
            {
                ApplyEffects(newItem, null); 
            }
                                         

            player.speed += (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));
            player.inventorySpeedModifier += newItem.weight * tAmount;
            player.speed -= (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));
        }
    }

    public void DropItem(Item Titem)
    {
        player.speed += (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));
        player.inventorySpeedModifier -= Titem.weight;
        player.speed -= (int)MathF.Floor(MathF.Pow(player.inventorySpeedModifier * scale, exponent));

        player.inventoryWeight -= Titem.weight;

        int equippedSlot = player.equippedItems.IndexOf(Titem);
        if (equippedSlot >= 0)
        {
            UnequipItem(equippedSlot);
        }

        if (Titem.type == ItemType.Artifact)
        {
            RemoveEffects(Titem);
        }

        if (Titem.amount <= 1)
        {
            player.ownedItems.Remove(Titem);
        }
        else
        {
            Titem.amount--;

            if (Titem.type == ItemType.Artifact)
            {
                ApplyEffects(Titem,null);
            }
        }
    }
    public void ApplyEffects(Item Titem, int? amount)
    {
        if (amount == null)
        {
            amount = Titem.amount;
        }
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "HP":{ int delta = Titem.stats["HP"] * Titem.amount;player.HP += delta;  if (player.HP > player.maxHP) player.HP = player.maxHP;  break;}
                case "maxHP": player.maxHP += Titem.stats["maxHP"] * Titem.amount; break;
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
                case "maxHP": player.maxHP -= Titem.stats["maxHP"] * Titem.amount; break;
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
            ApplyEffects(Titem,1);
            DropItem(Titem);
        }
        else
        {
            MainUI.WriteInMainArea("not done");
        }
    }

    public void UnequipItem(int slot)
    {
        Item itemToUnequip = player.equippedItems[slot]; 
        if (itemToUnequip != null)
        {
            MainUI.WriteInMainArea($"{itemToUnequip.name} unequipped from Slot {slot}.");

            RemoveEffects(itemToUnequip);
            player.equippedItems[slot] = null;
        }
    }
}

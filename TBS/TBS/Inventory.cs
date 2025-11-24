using Game.Class;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
public class Inventory
{
    private Player player;

    private int currentPage = 1; 
    private int itemsPerPage = 9;  
    private string searchTerm = ""; 
    private List<Item> filteredItems; // This will hold the items we are currently viewing

    const float exponent = 1.5f;
    const float scale = 0.1f;
    public int freeweight = 20;

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
                                   item.description.ToLower().Contains(searchTerm.ToLower()) ||
                                   item.GetDescription().ToLower().Contains(searchTerm.ToLower()))
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

            
            float excessWeight = MathF.Max(player.inventorySpeedModifier - freeweight, 0);
            MainUI.WriteInMainArea($"your items weigh {player.inventoryWeight} therefore your speed is being reduced by {(int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent))} \n");

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
            MainUI.WriteInMainArea("Type item number (1-9) to interact, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back to Main Menu");


            string inputString = Console.ReadKey(true).KeyChar.ToString().ToLower() ?? "";

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
                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue; 
            }

            Item selectedItem = pageItems[input - 1];

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"you've picked {selectedItem.name}");

            MainUI.WriteInMainArea("0 : cancel");
            MainUI.WriteInMainArea("1 : details");
            MainUI.WriteInMainArea("2 : drop");
            if (selectedItem.type == ItemType.equipment) MainUI.WriteInMainArea("3 : Equip/Unequip");
            if (selectedItem.type == ItemType.consumable && selectedItem.duration == 0) MainUI.WriteInMainArea("3 : consume");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("type out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int ik);
            if (!k || ik < 0 || ik > 3)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("my love would you please type a number this time\n ");

                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            else if(ik == 0)
            {
                continue; 
            }
            else if (ik == 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"you've picked {selectedItem.name}\n");
                MainUI.WriteInMainArea($"{selectedItem.GetDescription()}\n");

                MainUI.WriteInMainArea($"Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            else if (ik == 2)
            {
                MainUI.WriteInMainArea($"\nyou drop the {selectedItem.name}");
                DropItem(selectedItem,1);
            }
            else if (ik == 3 && selectedItem.type == ItemType.equipment)
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
            else if (ik == 3 && selectedItem.type == ItemType.consumable)
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
                RemoveEffects(existingItem,tAmount); 
            }

            player.inventoryWeight += existingItem.weight * tAmount;
            existingItem.amount += tAmount;

            if (existingItem.type == ItemType.Artifact)
            {
                ApplyEffects(existingItem, null); 
            }




            // remove the old speed modifier effect
            player.speed += (int)MathF.Floor(MathF.Pow(MathF.Max(player.inventorySpeedModifier - freeweight, 0) * scale, exponent));

            // update the modifier based on new weight
            player.inventorySpeedModifier += existingItem.weight * tAmount;

            // apply the new effect only if weight exceeds freeweight int
            float excessWeight = MathF.Max(player.inventorySpeedModifier - freeweight, 0);
            player.speed -= (int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent));



        }
        else if(templateItem.type == ItemType.equipment)
        {
            for (int i = 0; i < tAmount; i++)
            {
                Item newItem = new Item(templateItem); // Use the copy constructor

                player.ownedItems.Add(newItem);
                player.inventoryWeight += newItem.weight;

                // remove the old speed modifier effect
                player.speed += (int)MathF.Floor(MathF.Pow(MathF.Max(player.inventorySpeedModifier - freeweight, 0) * scale, exponent));

                // update the modifier based on new weight
                player.inventorySpeedModifier += newItem.weight;

                // apply the new effect only if weight exceeds freeweight int
                float excessWeight = MathF.Max(player.inventorySpeedModifier - freeweight, 0);
                player.speed -= (int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent));
            }
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


            // remove the old speed modifier effect
            player.speed += (int)MathF.Floor(MathF.Pow(MathF.Max(player.inventorySpeedModifier - freeweight, 0) * scale, exponent));

            // update the modifier based on new weight
            player.inventorySpeedModifier += newItem.weight * tAmount;

            // apply the new effect only if weight exceeds freeweight int
            float excessWeight = MathF.Max(player.inventorySpeedModifier - freeweight, 0);
            player.speed -= (int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent));

        }
    }

    public void DropItem(Item Titem, int quantity)
    {
        // remove the old speed modifier effect
        player.speed += (int)MathF.Floor(MathF.Pow(MathF.Max(player.inventorySpeedModifier - freeweight, 0) * scale, exponent));

        // update the modifier based on new weight
        player.inventorySpeedModifier -= Titem.weight * quantity;

        // apply the new effect only if weight exceeds freeweight int
        float excessWeight = MathF.Max(player.inventorySpeedModifier - freeweight, 0);
        player.speed -= (int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent));


        player.inventoryWeight -= Titem.weight * quantity;

        int equippedSlot = player.equippedItems.IndexOf(Titem);
        if (equippedSlot >= 0)
        {
            UnequipItem(equippedSlot);
        }

        if (Titem.type == ItemType.Artifact)
        {
            RemoveEffects(Titem,quantity);
        }

        if (Titem.amount <= 1)
        {
            player.ownedItems.Remove(Titem);
        }
        else
        {
            Titem.amount -= quantity;

            if (Titem.type == ItemType.Artifact)
            {
                ApplyEffects(Titem,null);
            }
        }
    }
    public void ApplyEffects(Item Titem, int? amount)
    {
        int namount;
        if (amount == null)
        {
            namount = Titem.amount;
        }
        else
        {
            namount = amount ?? 0;
        }
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "heal":{ int delta = Titem.stats["heal"] * namount;player.HP += delta;  if (player.HP > player.maxHP) player.HP = player.maxHP;  break;}
                case "damage": player.HP -= Titem.stats["damage"] * namount; break;
                case "maxHP": player.maxHP += Titem.stats["maxHP"] * namount; break;
                case "speed":player.speed += Titem.stats["speed"] * namount; break;
                case "armor":player.armor += Titem.stats["armor"] * namount; break;
                case "dodge":player.dodge += Titem.stats["dodge"] * namount; break;
                case "dodgeNegation":player.dodgeNegation += Titem.stats["dodgeNegation"] * namount;break;
                case "critChance":player.critChance += Titem.stats["critChance"] * namount; break;
                case "critDamage":player.critDamage += Titem.stats["critDamage"] * namount; break;
                case "stun":player.stun += Titem.stats["stun"] * namount; break;
                case "stunNegation":player.stunNegation += Titem.stats["stunNegation"] * namount; break;

            }
        }
    }
    public void RemoveEffects(Item Titem, int? amount)
    {
        int namount;
        if (amount == null)
        {
            namount = Titem.amount;
        }
        else
        {
            namount = amount ?? 0;
        }
        foreach (var stat in Titem.stats)
        {
            switch (stat.Key)
            {
                case "maxHP": player.maxHP -= Titem.stats["maxHP"] * namount; break;
                case "speed": player.speed -= Titem.stats["speed"] * namount; break;
                case "armor": player.armor -= Titem.stats["armor"] * namount; break;
                case "dodge": player.dodge -= Titem.stats["dodge"] * namount; break;
                case "dodgeNegation": player.dodgeNegation -= Titem.stats["dodgeNegation"] * namount; break;
                case "critChance": player.critChance -= Titem.stats["critChance"] * namount; break;
                case "critDamage": player.critDamage -= Titem.stats["critDamage"] * namount; break;
                case "stun": player.stun -= Titem.stats["stun"] * namount; break;
                case "stunNegation": player.stunNegation -= Titem.stats["stunNegation"] * namount; break;

            }
        }
    }
    public void Consume(Item Titem)
    {
        foreach (var effect in Titem.effects)
        {
            if (effect.targetType == "allEnemies")
            {
                Console.WriteLine($"{Titem.name} is an AoE move and requires ApplyToAll instead!");
            }
            else
            {
                if (effect.type == "heal")
                {
                    player.HP += effect.value;
                    if (player.HP > player.maxHP)
                    {
                        player.HP = player.maxHP;
                    }
                }
                else
                {
                    effect.Apply(player, player);
                }
            }
        }

        DropItem(Titem, 1);
    }

    public void UnequipItem(int slot)
    {
        Item itemToUnequip = player.equippedItems[slot]; 
        if (itemToUnequip != null)
        {
            MainUI.WriteInMainArea($"{itemToUnequip.name} unequipped from Slot {slot}.");

            RemoveEffects(itemToUnequip,1);
            player.equippedItems[slot] = null;
        }
    }
}

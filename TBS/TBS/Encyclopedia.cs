using Game.Class;
using System.ComponentModel;
using System.Reflection.Metadata;
using static System.Formats.Asn1.AsnWriter;
public class Encyclopedia
{
    private Player player;

    private int currentPage = 1;
    private int itemsPerPage = 8;
    private string searchTerm = "";
    private List<Item> filteredItems; // This will hold the items we are currently viewing

    const float exponent = 1.5f;
    const float scale = 0.1f;

    public Encyclopedia(Player p)
    {
        player = p;

        while (player.equippedItems.Count < 4)
        {
            player.equippedItems.Add(null);
        }

        filteredItems = player.ownedItems;
    }
    public void ShowEncyclopedia()
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

                MainUI.WriteInMainArea($"{i,-7}{item.name,-25} {item.weight,-5} {item.description,-20} {item.value}");
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
                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            Item selectedItem = pageItems[input - 1];

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"you're looking at {selectedItem.name}");

            MainUI.WriteInMainArea("0 : details");
            MainUI.WriteInMainArea("1 : cancel");


            var k = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int ik);
            if (!k || ik < 0 || ik > 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("my love would you please type a number this time\n ");

                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            else if (ik == 0)
            {
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea($"you've picked {selectedItem.name}");
                MainUI.WriteInMainArea($"{selectedItem.details}\n");

                MainUI.WriteInMainArea($"Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            else if (ik == 1)
            {
                //go back to the Encyclopedia here
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

    //I---------------------I
    //I find a way to       I
    //I add everything here I
    //I---------------------I

    /*public void AddItem(Item templateItem, int tAmount)
    {
        Item existingItem = player.ownedItems.FirstOrDefault(i => i.name == templateItem.name);

        if (existingItem != null && templateItem.type != ItemType.equipment)
        {
            if (existingItem.type == ItemType.Artifact)
            {
                RemoveEffects(existingItem, tAmount);
            }

            player.inventoryWeight += existingItem.weight * tAmount;
            existingItem.amount += tAmount;

            if (existingItem.type == ItemType.Artifact)
            {
                ApplyEffects(existingItem, null);
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
            player.speed += (int)MathF.Floor(MathF.Pow(MathF.Max(player.inventorySpeedModifier - 20, 0) * scale, exponent));

            // update the modifier based on new weight
            player.inventorySpeedModifier += existingItem.weight * tAmount;

            // apply the new effect only if weight exceeds 20
            float excessWeight = MathF.Max(player.inventorySpeedModifier - 20, 0);
            player.speed -= (int)MathF.Floor(MathF.Pow(excessWeight * scale, exponent));

        }
    }*/

}

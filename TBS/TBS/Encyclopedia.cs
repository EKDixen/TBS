using Game.Class;
using System.ComponentModel.Design;
using System.Numerics;
using System.Runtime.CompilerServices;


public class Encyclopedia
{
    private static int itemsearch = 0;
    private static int currentPage = 1;
    private static int itemsPerPage = 9;
    private static string searchTerm = "";
    private static List<Item> filteredItems; // This will hold the items we are currently viewing

    //  public static void Add

    public static void EncyclopediaLogic()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the Encyclopedia!");
            MainUI.WriteInMainArea($"What would you like to view?\n");
            MainUI.WriteInMainArea("1 : View items");
           // MainUI.WriteInMainArea("2 : View Enemies");
           // MainUI.WriteInMainArea("3 : View Locations");
            MainUI.WriteInMainArea("0 : Leave");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > /*3*/ 1 || input < 0)
            {
                MainUI.WriteInMainArea(" \nyou gotta type a number from 0-4");
                Thread.Sleep(1000);
                continue;
            }

            switch (input)
            {
                case 0:
                    Program.MainMenu();
                    return;
                case 1:
                    ViewItems();
                    break;
                case 2:
                    ViewItems();
                    break;
                case 3:
                    ViewItems();
                    break;
            }
            Program.SavePlayer();
        }
    }



    public static void ViewItems()
    {

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the Encyclopedia!");
            MainUI.WriteInMainArea($"What type of item would you like to view?\n");
            MainUI.WriteInMainArea("1 : View Consumables");
            MainUI.WriteInMainArea("2 : View Equipment");
            MainUI.WriteInMainArea("3 : View Artifacts");
            MainUI.WriteInMainArea("4 : View Materials");
            MainUI.WriteInMainArea("0 : Back");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 4 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, or 4");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                EncyclopediaLogic();
                return;
            }
            else if (input == 0) EncyclopediaLogic();
            else if (input == 1) itemsearch = 0;
            else if (input == 2) itemsearch = 1;
            else if (input == 3) itemsearch = 2;
            else if (input == 4) itemsearch = 3;
            break;
        }

        while (true)
        {
            // Update the filtered list based on the search term
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredItems = Program.player.knownItems.Where(item => item.type.Equals((ItemType)itemsearch))
                .ToList();       // full list

            }
            else
            {

                filteredItems = Program.player.knownItems
                    .Where(item => item.name.ToLower().Contains(searchTerm.ToLower()) ||
                                   item.description.ToLower().Contains(searchTerm.ToLower()) ||
                                   item.GetDescription().ToLower().Contains(searchTerm.ToLower()) ||
                                   item.type.Equals((ItemType)itemsearch))

                    .ToList();
            }


            // Calculate total pages and get the items for the current page
            int totalItems = filteredItems.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages; // Fix if we are on a page that no longer exists
            if (currentPage < 1) currentPage = 1;

            filteredItems.Sort((x, y) => string.Compare(x.name, y.name));

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
            if (itemsearch == 1)
            {
            MainUI.WriteInMainArea("nr     Name                   Value          Slot         Weight");
            }
            else
            {
            MainUI.WriteInMainArea("nr     Name                   Value          type         Weight");
            }

                MainUI.WriteInMainArea("----------------------------------------------------------------");
            int i = 0;
            foreach (var item in pageItems)
            {
                i++;
                if (itemsearch == 1)
                {
                MainUI.WriteInMainArea($"{i,-7}{item.name,-24} {item.value,-13} {item.equipmentType,-17} {item.weight}");
                }
                else
                {
                MainUI.WriteInMainArea($"{i,-7}{item.name,-24} {item.value,-10} {item.type,-17} {item.weight}");
                }
                
                
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
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("type out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int ik);
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
                continue;
            }
            else if (ik == 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"you've picked {selectedItem.name}\n");
                MainUI.WriteInMainArea($"\n{selectedItem.GetDescription()}\n");

                MainUI.WriteInMainArea($"Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            Program.SavePlayer();
            Program.MainMenu();
            break;
        }
    }

    private static void HandleSearch()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
        MainUI.WriteInMainArea("> ");
        searchTerm = Console.ReadLine()?.ToLower() ?? "";
        currentPage = 1; // ALWAYS reset to page 1 after a search
    }
}
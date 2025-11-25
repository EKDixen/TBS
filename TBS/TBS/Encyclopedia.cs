using Game.Class;


public static class Encyclopedia
{
public static void EncyclopediaLogic()
{
    while (true)
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Welcome to the Encyclopedia!");

        MainUI.WriteInMainArea("What would you like to view?");
        MainUI.WriteInMainArea("1 : View items");
        MainUI.WriteInMainArea("2 : View Enemies");
        MainUI.WriteInMainArea("3 : View Locations");
        MainUI.WriteInMainArea("0 : Leave");

        if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 3 || input < 0)
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


private static void ViewItems()
{

    while (true)
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Select an item to withdraw:");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("nr     Name                      Qty   Location");
        MainUI.WriteInMainArea("--------------------------------------------------");

        if (Program.player.bankItems.Count == 0)
        {
            MainUI.WriteInMainArea("Your bank is empty.");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("0 : Back");
        }
        else
        {
            for (int i = 0; i < Program.player.bankItems.Count; i++)
            {
                var (location, item) = Program.player.bankItems[i];
                MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-5} {location}");
            }
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("0 : Back");
        }

        string inputString = Console.ReadLine() ?? "";
        var n = int.TryParse(inputString, out int input);

        if (input == 0)
        {
            return;
        }

        if (!n || input < 1 || input > Program.player.bankItems.Count)
        {
            MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list.");
            Thread.Sleep(1000);
            continue;
        }

        var (loc, selectedItem) = Program.player.bankItems[input - 1];
        int quantity = 1;

        // If stackable, ask how many
        if (selectedItem.type != ItemType.equipment)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to withdraw? (Max: {selectedItem.amount})");
            string quantityString = Console.ReadLine() ?? "";
            var q = int.TryParse(quantityString, out quantity);

            if (!q || quantity < 1 || quantity > selectedItem.amount)
            {
                MainUI.WriteInMainArea("\nInvalid amount.");
                Thread.Sleep(1000);
                continue;
            }
        }


        Inventory.AddItem(selectedItem, quantity);

        // --- Remove from bank ---
        if (selectedItem.type == ItemType.equipment || selectedItem.amount == quantity)
        {
            // Remove the item completely from bank
            Program.player.bankItems.RemoveAt(input - 1);
        }
        else
        {
            // Just subtract the amount
            selectedItem.amount -= quantity;

        }

        MainUI.WriteInMainArea($"\nWithdrew {quantity}x {selectedItem.name}.");
        Thread.Sleep(1000);
    }
}
}
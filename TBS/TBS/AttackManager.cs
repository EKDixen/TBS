using Game.Class;
using System;
using System.Collections.Generic;

public class AttackManager
{
    private Player player;

    private int currentPage = 1;
    private int movesPerPage = 8;
    private string searchTerm = "";
    private List<Attack> filteredMoves; // This will hold the moves we are currently viewing
    public AttackManager(Player p)
    {
        player = p;

        while (player.equippedAttacks.Count < 4)
        {
            player.equippedAttacks.Add(null);
        }

        filteredMoves = player.ownedAttacks;
    }

    public void LearnAttack(Attack attack)
    {
        if (!player.ownedAttacks.Contains(attack))
        {
            player.ownedAttacks.Add(attack);
            MainUI.WriteInMainArea($"{player.name} learned {attack.name}!");
        }
        else
        {
            MainUI.WriteInMainArea($"{player.name} already knows {attack.name}.");
        }
    }

    public void EquipAttack(Attack attack, int slot)
    {
        if (!player.ownedAttacks.Contains(attack))
        {
            MainUI.WriteInMainArea($"{player.name} doesn’t know {attack.name} yet!");
            return;
        }

        if (slot < 1 || slot > 4)
        {
            MainUI.WriteInMainArea("Invalid slot. Choose between 1 and 4.");
            return;
        }

        player.equippedAttacks[slot - 1] = attack; // replace whatever was in the slot
        MainUI.WriteInMainArea($"{attack.name} equipped into Slot {slot}!");
    }

    public void UnequipAttack(int slot)
    {
        if (slot < 1 || slot > 4)
        {
            MainUI.WriteInMainArea("Invalid slot. Choose between 1 and 4.");
            return;
        }

        if (player.equippedAttacks[slot - 1] != null)
        {
            MainUI.WriteInMainArea($"{player.equippedAttacks[slot - 1].name} unequipped from Slot {slot}.");
            player.equippedAttacks[slot - 1] = null;
        }
        else
        {
            MainUI.WriteInMainArea($"Slot {slot} is already empty.");
        }
    }

    // Show moves menu for equipping/unequipping/viewing ig
    public void ShowMovesMenu()
    {
        while (true)
        {
            // Update the filtered list based on the search term
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredMoves = player.ownedAttacks; // full list
            }
            else
            {
                filteredMoves = player.ownedAttacks
                    .Where(Attack => Attack.name.ToLower().Contains(searchTerm.ToLower()) ||
                                   Attack.GetDescription().ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }

            // Calculate total pages and get the items for the current page
            int totalMoves = filteredMoves.Count;
            int totalPages = (int)Math.Ceiling((double)totalMoves / movesPerPage);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages; // Fix if we are on a page that no longer exists
            if (currentPage < 1) currentPage = 1;

            List<Attack> pageMoves = filteredMoves
                .Skip((currentPage - 1) * movesPerPage) // Skip items on previous pages
                .Take(movesPerPage)                     // Get just the items for this page
                .ToList();

            MainUI.ClearMainArea();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                MainUI.WriteInMainArea($"\nShowing results for: \"{searchTerm}\"");
            }

            MainUI.WriteInMainArea("\n--- Moves Menu ---");
            for (int i = 0; i < pageMoves.Count; i++)
            {
                Attack atk = pageMoves[i];

                // check if equipped
                int slotIndex = pageMoves.IndexOf(atk);
                string equippedInfo = slotIndex >= 0 ? $"(Slot {slotIndex + 1})" : "";

                MainUI.WriteInMainArea($"{i + 1}. {atk.name} {equippedInfo}");
                MainUI.WriteInMainArea($"   -> {atk.GetDescription()}");
            }
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {currentPage} of {totalPages} ---");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Type move number (1-8) to interact, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back to Main Menu");

            string inputString = Console.ReadLine()?.ToLower() ?? "";

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

            if (!n || input < 1 || input > pageMoves.Count)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a usable number *from this page* ");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("-press Enter to continue-");
                Console.ReadLine();
                continue;
            }

            Attack chosen = pageMoves[input - 1];




            // if already equipped, unequip
            int equippedSlot = player.equippedAttacks.IndexOf(chosen);
            if (equippedSlot >= 0)
            {
                MainUI.WriteInMainArea($"{chosen.name} is currently in Slot {equippedSlot + 1}. Unequipping...");
                player.equippedAttacks[equippedSlot] = null;

                MainUI.WriteInMainArea("\n-press Enter to continue-");
                Console.ReadLine();
                continue;
            }

            // equip
            MainUI.WriteInMainArea("Choose a slot (1-4) or 0 to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot < 0 || slot > 4) continue;
            if (slot == 0) continue;

            player.equippedAttacks[slot - 1] = chosen;
            MainUI.WriteInMainArea($"{chosen.name} equipped into Slot {slot}!");
        }
        Program.MainMenu();
    }
    private void HandleSearch()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
        MainUI.WriteInMainArea("> ");
        searchTerm = Console.ReadLine()?.ToLower() ?? "";
        currentPage = 1; // ALWAYS reset to page 1 after a search
    }
}

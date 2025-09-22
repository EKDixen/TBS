using Game.Class;
using System;
using System.Collections.Generic;

public class AttackManager
{
    private Player player;

    public AttackManager(Player p)
    {
        player = p;

        while (player.equippedAttacks.Count < 4)
        {
            player.equippedAttacks.Add(null);
        }
    }

    public void LearnAttack(Attack attack)
    {
        if (!player.ownedAttacks.Contains(attack))
        {
            player.ownedAttacks.Add(attack);
            Console.WriteLine($"{player.name} learned {attack.name}!");
        }
        else
        {
            Console.WriteLine($"{player.name} already knows {attack.name}.");
        }
    }

    public void EquipAttack(Attack attack, int slot)
    {
        if (!player.ownedAttacks.Contains(attack))
        {
            Console.WriteLine($"{player.name} doesn’t know {attack.name} yet!");
            return;
        }

        if (slot < 1 || slot > 4)
        {
            Console.WriteLine("Invalid slot. Choose between 1 and 4.");
            return;
        }

        player.equippedAttacks[slot - 1] = attack; // replace whatever was in the slot
        Console.WriteLine($"{attack.name} equipped into Slot {slot}!");
    }

    public void UnequipAttack(int slot)
    {
        if (slot < 1 || slot > 4)
        {
            Console.WriteLine("Invalid slot. Choose between 1 and 4.");
            return;
        }

        if (player.equippedAttacks[slot - 1] != null)
        {
            Console.WriteLine($"{player.equippedAttacks[slot - 1].name} unequipped from Slot {slot}.");
            player.equippedAttacks[slot - 1] = null;
        }
        else
        {
            Console.WriteLine($"Slot {slot} is already empty.");
        }
    }

    // Show moves menu for equipping/unequipping/viewing ig
    public void ShowMovesMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Moves Menu ---");
            for (int i = 0; i < player.ownedAttacks.Count; i++)
            {
                Attack atk = player.ownedAttacks[i];

                // check if equipped
                int slotIndex = player.equippedAttacks.IndexOf(atk);
                string equippedInfo = slotIndex >= 0 ? $"(Slot {slotIndex + 1})" : "";

                Console.WriteLine($"{i + 1}. {atk.name} {equippedInfo}");
                Console.WriteLine($"   -> {atk.GetDescription()}");
            }
            Console.WriteLine("0. Cancel");
            Console.WriteLine("------------------");

            Console.Write("Choose a move number to equip/unequip (0 to exit): ");
            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            if (choice == 0) break; // cancel

            if (choice < 1 || choice > player.ownedAttacks.Count)
            {
                Console.WriteLine("Invalid choice.");
                continue;
            }

            Attack chosen = player.ownedAttacks[choice - 1];

            // if already equipped, unequip
            int equippedSlot = player.equippedAttacks.IndexOf(chosen);
            if (equippedSlot >= 0)
            {
                Console.WriteLine($"{chosen.name} is currently in Slot {equippedSlot + 1}. Unequipping...");
                player.equippedAttacks[equippedSlot] = null;
                continue;
            }

            // equip
            Console.Write("Choose a slot (1-4) or 0 to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot < 0 || slot > 4) continue;
            if (slot == 0) continue;

            player.equippedAttacks[slot - 1] = chosen;
            Console.WriteLine($"{chosen.name} equipped into Slot {slot}!");
        }
        Program.MainMenu();
    }
}

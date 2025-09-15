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
        Console.WriteLine($"{attack.name} equipped into slot {slot}!");
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
            Console.WriteLine($"{player.equippedAttacks[slot - 1].name} unequipped from slot {slot}.");
            player.equippedAttacks[slot - 1] = null;
        }
        else
        {
            Console.WriteLine($"Slot {slot} is already empty.");
        }
    }

    public void ShowOwnedAttacks()
    {
        Console.WriteLine($"{player.name}'s known attacks:");
        foreach (var atk in player.ownedAttacks)
        {
            Console.WriteLine($"- {atk.name}");
        }
    }

    public void ShowEquippedAttacks()
    {
        Console.WriteLine($"{player.name}'s equipped attacks:");
        for (int i = 0; i < player.equippedAttacks.Count; i++)
        {
            string atkName = player.equippedAttacks[i]?.name ?? "Empty";
            Console.WriteLine($"Slot {i + 1}: {atkName}");
        }
    }
}

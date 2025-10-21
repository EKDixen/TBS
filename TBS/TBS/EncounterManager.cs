using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;

public class EncounterManager
{
    private Player player;
    private Random rng = new Random();

    public EncounterManager(Player player)
    {
        this.player = player;
    }
    public void ProcessTravelEncounters(Location from, Location to, bool isExploring = false)
    {
        List<Encounter> encounters = Encounter.StartTravelEncounters(from, to);

        if (encounters.Count == 0)
        {
            MainUI.WriteInMainArea("\nYour journey was peaceful.");
            MainUI.WriteInMainArea("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        string journeyType = isExploring ? "exploration" : "journey";
        MainUI.WriteInMainArea($"\n=== {encounters.Count} encounter(s) occurred during your {journeyType}! ===");
        MainUI.WriteInMainArea("Press Enter to continue...");
        Console.ReadLine();

        foreach (var encounter in encounters)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("╔════════════════════════════════════════╗");
            MainUI.WriteInMainArea("║          ENCOUNTER                     ║");
            MainUI.WriteInMainArea("╚════════════════════════════════════════╝");
            MainUI.WriteInMainArea("");
            
            encounter.Execute(player);

            Program.CheckPlayerDeath();
            if (!player.IsAlive())
            {
                return;
            }
        }

        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("All encounters resolved!");
        MainUI.WriteInMainArea("Press Enter to continue...");
        Console.ReadLine();
    }


    public Encounter CreateCustomEncounter(string name, string description, List<Enemy> enemies = null, Action<Player> onEncounter = null, EncounterType type = EncounterType.Event)
    {
        bool isEnemyEncounter = enemies != null && enemies.Count > 0;
        return new Encounter(name, isEnemyEncounter, description, enemies, onEncounter, type);
    }

    public void DisplayLocationEncounterInfo(Location location)
    {
        if (location.possibleEncounters == null || location.possibleEncounters.Count == 0)
        {
            MainUI.WriteInMainArea($"{location.name} has no encounters.");
            return;
        }

        MainUI.WriteInMainArea($"\n=== Encounters in {location.name} ===");
        int total = location.possibleEncounters.Values.Sum();
        
        foreach (var kvp in location.possibleEncounters.OrderByDescending(x => x.Value))
        {
            double percentage = (kvp.Value / (double)total) * 100;
            string typeIcon = GetEncounterTypeIcon(kvp.Key.Type);
            MainUI.WriteInMainArea($"{typeIcon} {kvp.Key.Name}: {percentage:F1}% ({kvp.Key.Type})");
        }
    }

    private string GetEncounterTypeIcon(EncounterType type)
    {
        return type switch
        {
            EncounterType.Combat => "⚔",
            EncounterType.Treasure => "💰",
            EncounterType.Event => "📜",
            EncounterType.Merchant => "🛒",
            EncounterType.Trap => "⚠",
            EncounterType.Mystery => "❓",
            _ => "•"
        };
    }
}

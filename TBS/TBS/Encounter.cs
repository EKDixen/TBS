﻿using Game.Class;
using System.Diagnostics;

public class Encounter
{
    public string Name;
    public bool IsEnemyEncounter;
    public string Description;
    public List<Enemy> Enemies;
    public Action<Player> OnEncounter;
    public EncounterType Type;

    public Encounter(string name, bool isEnemyEncounter, string description, List<Enemy> enemies = null, Action<Player> onEncounter = null, EncounterType type = EncounterType.Event)
    {
        Name = name;
        IsEnemyEncounter = isEnemyEncounter;
        Description = description;
        Enemies = enemies ?? new List<Enemy>();
        OnEncounter = onEncounter;
        Type = type;
    }
    public Encounter() { }

    private static Random rng = new Random();

    public static List<Encounter> StartTravelEncounters(Location a, Location b)
    {
        Dictionary<Encounter, int> possibleEncounters = GetPossibleEncounters(a, b);

        int encounterThreshold = 1000;
        int encounterRoll;

        int total = possibleEncounters.Values.Sum();
        var picks = new List<Encounter>();

        for (encounterRoll = rng.Next(0, 1001); encounterThreshold > encounterRoll; encounterThreshold -= Program.player.luck)
        {
            int r = rng.Next(0, total);
            int accum = 0;
                
            foreach (var kv in possibleEncounters)
            {
                accum += kv.Value;
                if (r < accum)
                {
                    picks.Add(kv.Key);
                    break;
                }
            }
        }
        return picks;
    }

    public static Dictionary<Encounter, int> GetPossibleEncounters(Location a, Location b)
    {
        var combined = new Dictionary<Encounter, int>();
        void addAll(Dictionary<Encounter, int> src)
        {
            if (src == null) return;
            foreach (var kv in src)
            {
                if (combined.ContainsKey(kv.Key)) combined[kv.Key] += kv.Value;
                else combined[kv.Key] = kv.Value;
            }
        }
        addAll(a.possibleEncounters);
        addAll(b.possibleEncounters);

        if (combined.Count == 0)
        {
            Console.WriteLine("No encounters in these zones.");
            return null;
        }

        return combined;
    }

    public void Execute(Player player)
    {
        MainUI.WriteInMainArea($"\n{Description}");
        
        if (IsEnemyEncounter && Enemies != null && Enemies.Count > 0)
        {
            MainUI.WriteInMainArea("\nPrepare for battle!");
            Console.ReadLine();
            
            List<Enemy> combatEnemies = new List<Enemy>();
            foreach (var enemy in Enemies)
            {
                var newEnemy = new Enemy(
                    enemy.name, enemy.level, enemy.exp, enemy.HP, enemy.DMG,
                    enemy.speed, enemy.armor, enemy.dodge, enemy.dodgeNegation,
                    enemy.critChance, enemy.critDamage, enemy.stun, enemy.stunNegation, enemy.money
                );
                newEnemy.attacks = new List<Attack>(enemy.attacks);
                newEnemy.maxHP = enemy.HP;
                combatEnemies.Add(newEnemy);
            }
            
            CombatManager combat = new CombatManager(player, combatEnemies);
            combat.StartCombat();
        }
        else if (OnEncounter != null)
        {
            OnEncounter(player);
            MainUI.WriteInMainArea("\nPress Enter to continue...");
            Console.ReadLine();
        }
        else
        {
            MainUI.WriteInMainArea("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}

public enum EncounterType
{
    Event,      // Random event (find items, lose items, etc.)
    Combat,     // Enemy encounter
    Treasure,   // Find treasure/loot
    Merchant,   // Random merchant
    Trap,       // Trap or hazard
    Mystery     // Unknown/special event
}
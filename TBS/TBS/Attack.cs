using System;
using System.Collections.Generic;

public class Attack
{
    public string name;
    public List<AttackEffect> effects;

    public Attack() { } // Deserialization


    public Attack(string Tname, List<AttackEffect> Teffects)
    {
        name = Tname;
        effects = Teffects;
    }

    public void Apply(Combatant attacker, Combatant defender)
    {
        Console.WriteLine($"{attacker.name} uses {name} on {defender.name}!");
        foreach (var effect in effects)
        {
            if (effect.targetType == "allEnemies")
                Console.WriteLine($"{name} is an AoE move and requires ApplyToAll instead!");
            else
                effect.Apply(attacker, defender);
        }
    }

    public void ApplyToAll(Combatant attacker, List<Combatant> defenders)
    {
        Console.WriteLine($"{attacker.name} uses {name} on ALL enemies!");
        foreach (var effect in effects)
        {
            if (effect.targetType == "allEnemies")
                effect.ApplyToAll(attacker, defenders);
            else
                Console.WriteLine($"{effect.type} skipped (not marked as allEnemies).");
        }
    }

    public string GetDescription()
    {
        List<string> parts = new List<string>();
        foreach (var effect in effects)
        {
            string target = effect.targetType == "self" ? "yourself" :
                            effect.targetType == "allEnemies" ? "all enemies" : "an enemy";

            string desc = effect.type switch
            {
                "damage" => $"Deal {effect.value} damage to {target}",
                "heal" => $"Heal {effect.value} HP",
                "armor" => $"Increase armor by {effect.value}{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "critChance" => $"Increase crit chance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "critDamage" => $"Increase crit damage by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "dodge" => $"Increase dodge by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "dodgeNegation" => $"Increase dodge resistance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "stun" => $"Increase stun chance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                "stunNegation" => $"Increase stun resistance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns" : "")}",
                _ => $"{effect.type} {effect.value} {(effect.duration > 0 ? $"({effect.duration} turns)" : "")}"
            };

            parts.Add(desc);
        }

        return string.Join(", ", parts);
    }
}

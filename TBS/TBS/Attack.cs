using System;
using System.Collections.Generic;

public class Attack
{
    public string name;
    public List<AttackEffect> effects;

    public Attack(string Tname, List<AttackEffect> Teffects)
    {
        name = Tname;
        effects = Teffects;
    }

    // Single target
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

    // AOE
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
}

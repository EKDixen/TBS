using Game.Class;
using System.Collections.Generic;

// Enemy constructor order:
// name, level, exp, HP, DMG, speed, armor, dodge, dodgeNegation, critChance, critDamage, stun, stunNegation, money
public static class EnemyLibrary
{
    public static Enemy Thug = new Enemy("Thug", 2, 10, 30, 8, 8, 2, 5, 5, 5, 150, 0, 0, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands
        }
    };

    public static Enemy VampireSpawn = new Enemy("Vampire Spawn", 5, 25, 50, 12, 12, 5, 10, 10, 10, 160, 5, 5, 25)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.VampiricSlash
        }
    };
}
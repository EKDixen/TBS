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

    public static Enemy Goblin = new Enemy("Goblin", 2, 5, 5, 20, 16, 0, 25, 0, 10, 100, 5, 0, 5)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        }
    };
    public static Enemy SkeletonWarrior = new Enemy("Skeleton Warrior", 10, 50, 50, 30, 5, 1, 1, 0, 20, 50, 10, 0, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash
        }
    };

    public static Enemy GhostlyApparition = new Enemy("Ghostly Apparition", 10, 70, 20, 20, 8, 0, 20, 10, 5, 0, 20, 0, 15)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch
        }
    };

}
using Game.Class;
using System.Collections.Generic;

// Enemy constructor order:
// name, level, exp, HP, DMG, speed, armor, dodge, dodgeNegation, critChance, critDamage, stun, stunNegation, money
public static class EnemyLibrary
{
    #region Basic/Starter Enemies
    public static Enemy Thug = new Enemy("Thug", 1, 10, 30,  8, 2, 5, 5, 5, 150, 0, 0, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 100 }
        }
    };

    public static Enemy Goblin = new Enemy("Goblin", 1, 5, 5, 16, 0, 25, 0, 10, 100, 5, 0, 5)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 70 },
            { AttackLibrary.ThrowHands, 30 }
        }
    };
    #endregion

    #region Coastal Alliance Enemies
    public static Enemy SeaBandit = new Enemy("Sea Bandit", 2, 15, 35, 9, 2, 6, 7, 6, 130, 5, 3, 15)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 55 },
            { AttackLibrary.ThrowHands, 45 }
        }
    };

    public static Enemy Smuggler = new Enemy("Smuggler",3, 25, 45, 10, 3, 8, 9, 8, 150, 8, 5, 22)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 65 },
            { AttackLibrary.ThrowHands, 35 }
        }
    };
    public static Enemy Healer = new Enemy("Healer", 3, 25, 45, 10, 3, 8, 9, 8, 150, 8, 5, 22)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.GroupHeal
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.GroupHeal, 100 },
        }
    };
    #endregion

    #region Greenwood Territories Enemies
    public static Enemy ForestSpider = new Enemy("Forest Spider", 4, 20, 35, 8, 2, 5, 10, 6, 110, 8, 4, 12)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 75 },
            { AttackLibrary.Bite, 25 }
        }
    };

    public static Enemy DireWolf = new Enemy("Dire Wolf", 3, 35, 50, 12, 3, 8, 10, 8, 140, 12, 6, 18)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 85 },
            { AttackLibrary.Bite, 15 }
        }
    };
    #endregion

    #region Fallen Kingdom Enemies
    public static Enemy VampireSpawn = new Enemy("Vampire Spawn", 3, 25, 40, 12, 0, 10, 10, 10, 160, 5, 5, 12)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.VampiricSlash, 100 }
        }
    };

    public static Enemy GhostlyApparition = new Enemy("Ghostly Apparition", 6, 30, 25, 8, 0, 20, 10, 5, 0, 200, 0, 15)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.EtherealTouch, 100 }
        }
    };

    public static Enemy SkeletonWarrior = new Enemy("Skeleton Warrior", 4, 25, 50, 5, 1, 1, 0, 20, 50, 100, 300, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 100 }
        }
    };

    public static Enemy KingdomGuard = new Enemy("Kingdom Guard", 5, 30, 55, 10, 2, 12, 6, 8, 130, 8, 6, 18)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 65 },
            { AttackLibrary.ThrowHands, 35 }
        }
    };

    public static Enemy CorruptedKnight = new Enemy("Corrupted Knight", 6, 70, 90, 14, 1, 18, 8, 12, 140, 15, 15, 40)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 50 },
            { AttackLibrary.VampiricSlash, 50 }
        }
    };

    public static Enemy RuinedGolem = new Enemy("Ruined Golem", 7, 120, 150, 20, 0, 25, 3, 15, 100, 20, 20, 60)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 100 }
        }
    };
    #endregion

    #region Frostborn Dominion Enemies
    public static Enemy IceWolf = new Enemy("Ice Wolf", 8, 40, 60, 10, 3, 8, 8, 8, 120, 10, 5, 20)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 60 },
            { AttackLibrary.Bite, 40 }
        }
    };

    public static Enemy FrostTroll = new Enemy("Frost Troll", 12, 80, 120, 15, 1, 15, 5, 12, 110, 15, 10, 35)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands,
            AttackLibrary.Snowball
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 65 },
            { AttackLibrary.Snowball, 35 }
        }
    };

    public static Enemy IceMage = new Enemy("Ice Mage", 10, 60, 50, 12, 2, 10, 12, 10, 150, 20, 8, 30)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch,
            AttackLibrary.GroupHeal
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.EtherealTouch, 40 },
            { AttackLibrary.GroupHeal, 60 }
        }
    };

    public static Enemy SnowWraith = new Enemy("Snow Wraith", 14, 100, 80, 18, 4, 12, 15, 15, 180, 25, 12, 50)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch,
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.EtherealTouch, 55 },
            { AttackLibrary.VampiricSlash, 45 }
        }
    };
    #endregion

}
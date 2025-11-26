using System.Collections.Generic;

public class Combatant
{
    //Stats
    public string name;
    public int level;
    public int exp;
    public int HP;
    public int maxHP;
    public int speed;
    public int armor;

    // Extra stats
    public int dodge;
    public int dodgeNegation;
    public int critChance;
    public int critDamage;
    public int stun;
    public int stunNegation;

    public int money;

    public double ActionGauge;
    public bool IsPlayer;
    public bool IsAlly;

    public List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    public List<DamageOverTimeEffect> damageOverTimeEffects = new List<DamageOverTimeEffect>();
    public List<HealOverTimeEffect> healOverTimeEffects = new List<HealOverTimeEffect>();

    public Combatant() { } // Deserialize

    public bool IsAlive()
    {
        return HP > 0;
    }
}

public class ActiveEffect
{
    public string type;
    public int value;
    public int remainingTurns;

    public ActiveEffect(string type, int value, int remainingTurns)
    {
        this.type = type;
        this.value = value;
        this.remainingTurns = remainingTurns;
    }
}

public class DamageOverTimeEffect
{
    public int damagePerTurn;
    public int remainingTurns;
    public string sourceName; // name of the attack that caused it

    public DamageOverTimeEffect(int damagePerTurn, int remainingTurns, string sourceName)
    {
        this.damagePerTurn = damagePerTurn;
        this.remainingTurns = remainingTurns;
        this.sourceName = sourceName;
    }
}

public class HealOverTimeEffect
{
    public int healPerTurn;
    public int remainingTurns;
    public string sourceName;

    public HealOverTimeEffect(int healPerTurn, int remainingTurns, string sourceName)
    {
        this.healPerTurn = healPerTurn;
        this.remainingTurns = remainingTurns;
        this.sourceName = sourceName;
    }
}
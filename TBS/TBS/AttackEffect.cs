public class AttackEffect
{
    public string type;         // "damage", "heal", "critChance", etc.
    public int value;           // +/- amount
    public int duration;        // 0 = instant, >0 = lasts until X turns
    public string targetType;   // "self", "enemy", or "allEnemies"

    public AttackEffect(string Ttype, int TValue, int Tduration, string TtargetType)
    {
        type = Ttype;
        value = TValue;
        duration = Tduration;
        targetType = TtargetType;
    }

    // Single target
    public void Apply(Combatant attacker, Combatant defender)
    {
        Combatant target = (targetType == "self") ? attacker : defender;

        switch (type)
        {
            case "damage": target.HP -= value; break;
            case "heal": target.HP += value; break;
            case "dodge": target.dodge += value; break;
            case "dodgeNegation": target.dodgeNegation += value; break;
            case "critChance": target.critChance += value; break;
            case "critDamage": target.critDamage += value; break;
            case "armor": target.armor += value; break;
            case "stun": target.stun += value; break;
            case "stunNegation": target.stunNegation += value; break;
            default: Console.WriteLine($"Unknown effect: {type}"); break;
        }
    }

    // AOE
    public void ApplyToAll(Combatant attacker, List<Combatant> defenders)
    {
        foreach (var defender in defenders)
        {
            if (targetType == "enemy") // normal targeting
            {
                Apply(attacker, defender);
            }
        }
    }
}

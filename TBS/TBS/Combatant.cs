public class Combatant
{
    //Stats
    public string name;
    public int level;
    public int exp;
    public int HP;
    public int DMG;
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

    public Combatant() { } // Deserialize
}

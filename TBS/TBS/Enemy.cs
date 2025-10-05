public class Enemy : Combatant
{
    public List<Attack> attacks = new List<Attack>();

    public Enemy(string TenemyName, int Tlevel, int Texp, int THP, int TDMG, int Tspeed, int Tarmor,
        int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun,
        int TstunNegation, int Tmoney)
    {
        name = TenemyName;
        level = Tlevel;
        exp = Texp;
        HP = THP;
        DMG = TDMG;
        speed = Tspeed;
        armor = Tarmor;
        dodge = Tdodge;
        dodgeNegation = TdodgeNegation;
        critChance = Tcritchance;
        critDamage = TcritDamage;
        stun = Tstun;
        stunNegation = TstunNegation;
        money = Tmoney;
    }

    public Enemy() { }
}
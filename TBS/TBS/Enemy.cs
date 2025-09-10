using System.Diagnostics;

public class Enemy
{
    //stats 
    public string enemyName;
    public int level;
    public int exp;
    public int HP;
    public int DMG;
    public int speed;
    public int armor;

    //Extra stats (regnet i %)
    public int dodge;
    public int dodgeNegation;
    public int critChance;
    public int critDamage;
    public int stun;
    public int stunNegation;

    public int[] location;
    public int money;
    public Enemy(string TenemyName, int Tlevel, int Texp, int THP, int TDMG, int Tspeed, int Tarmor,
        int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun, int TstunNegation, int[] Tlocation, int Tmoney)
    {
        enemyName = TenemyName;
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
        location = Tlocation;
        money = Tmoney;
    }
}
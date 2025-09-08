using System.Diagnostics;

class Player
{
    //stats 
    string playerName;
    string password;
    string playerClass;
    int level;
    int exp;
    int HP;
    int DMG;
    int speed;
    int armor;

    //Extra stats (regnet i %)
    int dodge;
    int dodgeNegation;
    int critChance;
    int critDamage;
    int stun;
    int stunNegation;

    int luck;

    public Player(string TplayerName, string Tpassword, string TplayerClass, int Tlevel, int Texp, int THP, int TDMG, int Tspeed, int Tarmor, 
        int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun, int TstunNegation, int Tluck)
    {
        playerName = TplayerName;
        password = Tpassword;
        playerClass = TplayerClass;
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
        luck = Tluck;
    }
}
using System.Diagnostics;

public class Player
{
    //stats 
    public string playerName;
    public string password;
    public string playerClass;
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

    public int money;
    public int luck;

    public Location currentLocation;
    public List<Location> knownLocations = new List<Location>();
    Location starterTown = new Location(true, "StarterTown", new System.Numerics.Vector2(0, 0));

    public Player() { } // IK SLET (Brugt til saving)

    public Player(string TplayerName, string Tpassword, string TplayerClass, int Tlevel, int Texp, int THP, int TDMG, int Tspeed, int Tarmor,
        int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun, int TstunNegation, Location Tlocation, int Tmoney, int Tluck)
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
        currentLocation = Tlocation;
        money = Tmoney;
        luck = Tluck;

        knownLocations.Add(starterTown);
    }
}

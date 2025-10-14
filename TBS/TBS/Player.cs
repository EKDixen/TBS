using System.Diagnostics;

public class Player : Combatant
{
    public string password;
    public string playerClass;
    public int luck;

    public Location currentLocation;
    public List<Location> knownLocations = new List<Location>();

    public List<Item> ownedItems = new List<Item>();

    public List<Item> equippedItems = new List<Item>(4);

    public List<Attack> ownedAttacks = new List<Attack>();

    public List<Attack> equippedAttacks = new List<Attack>(4);

    public Player() { } //Deserialize

    public Player(string TplayerName, string Tpassword, string TplayerClass, int Tlevel, int Texp, int THP, int TDMG,
        int Tspeed, int Tarmor, int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun,
        int TstunNegation, Location Tlocation, int Tmoney, int Tluck)
    {
        name = TplayerName;
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

        knownLocations.Add(LocationLibrary.starterTown);
        currentLocation = LocationLibrary.starterTown;
    }
}

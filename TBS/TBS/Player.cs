using System.Data;
using System.Diagnostics;

public class Player : Combatant
{
    public string password;
    public Class playerClass;
    public int luck;
    public bool isDead = false;

    public string currentLocation;

    public List<string> knownLocationnames = new List<string>();

    public List<(string location, Item item)> bankItems = new List<(string location, Item item)>();
    public int bankMoney;

    public List<Item> ownedItems = new List<Item>();

    public List<Item> equippedItems = new List<Item>(4);

    public float inventoryWeight = 0;
    public float inventorySpeedModifier = 0;

    public List<Attack> ownedAttacks = new List<Attack>();

    public List<Attack> equippedAttacks = new List<Attack>(4);

    public Player() { } //Deserialize

    public Player(string TplayerName, string Tpassword, Class TplayerClass, int Tlevel, int Texp, int THP, int TmaxHP,
        int Tspeed, int Tarmor, int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun,
        int TstunNegation, Location Tlocation, int Tmoney, int Tluck)
    {
        name = TplayerName;
        password = Tpassword;
        playerClass = TplayerClass;
        level = Tlevel;
        exp = Texp;
        HP = THP;
        maxHP = TmaxHP;
        speed = Tspeed;
        armor = Tarmor;
        dodge = Tdodge;
        dodgeNegation = TdodgeNegation;
        critChance = Tcritchance;
        critDamage = TcritDamage;
        stun = Tstun;
        stunNegation = TstunNegation;

        money = Tmoney;
        luck = Tluck;

        knownLocationnames.Add(LocationLibrary.Maplecross.name);
        currentLocation = LocationLibrary.Maplecross.name;
    }
}

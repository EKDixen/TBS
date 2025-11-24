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

    // Normal inventory weight & encumbrance
    public int inventoryWeight = 0;
    public int inventorySpeedModifier = 0;

    // Material bag: stores crafting materials separately from normal inventory weight
    public List<Item> materialItems = new List<Item>();

    // Current material load in abstract units (not normal weight)
    public int currentMaterialLoad = 0;

    // Base capacity without backpacks; start with 100 units so players can carry some mats even without a pack
    public int baseMaterialCapacity = 100;

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

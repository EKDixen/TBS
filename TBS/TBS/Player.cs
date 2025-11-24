using System.Data;
using System.Diagnostics;

public class Player : Combatant
{
    public string password;
    public Class playerClass;
    public int luck;

    public int baseMaxHP;
    public int baseSpeed;
    public int baseArmor;
    public int baseDodge;
    public int baseDodgeNegation;
    public int baseCritChance;
    public int baseCritDamage;
    public int baseStun;
    public int baseStunNegation;
    public int baseLuck;
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

        baseMaxHP        = TmaxHP;
        baseSpeed        = Tspeed;
        baseArmor        = Tarmor;
        baseDodge        = Tdodge;
        baseDodgeNegation= TdodgeNegation;
        baseCritChance   = Tcritchance;
        baseCritDamage   = TcritDamage;
        baseStun         = Tstun;
        baseStunNegation = TstunNegation;
        baseLuck         = Tluck;

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
        luck = Tluck;

        money = Tmoney;

        knownLocationnames.Add(LocationLibrary.Maplecross.name);
        currentLocation = LocationLibrary.Maplecross.name;
    }

    public void RecalculateStats()
    {
        int levelsAbove1 = Math.Max(level - 1, 0);
        var c = playerClass;

        maxHP        = baseMaxHP        + c.TmaxHP        * levelsAbove1;
        speed        = baseSpeed        + c.Tspeed        * levelsAbove1;
        armor        = baseArmor        + c.Tarmor        * levelsAbove1;
        dodge        = baseDodge        + c.Tdodge        * levelsAbove1;
        dodgeNegation= baseDodgeNegation+ c.TdodgeNegation* levelsAbove1;
        critChance   = baseCritChance   + c.Tcritchance   * levelsAbove1;
        critDamage   = baseCritDamage   + c.TcritDamage   * levelsAbove1;
        stun         = baseStun         + c.Tstun         * levelsAbove1;
        stunNegation = baseStunNegation + c.TstunNegation * levelsAbove1;
        luck         = baseLuck         + c.Tluck         * levelsAbove1;

        if (HP > maxHP) HP = maxHP;
    }
}

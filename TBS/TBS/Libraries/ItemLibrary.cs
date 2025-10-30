public static class ItemLibrary
{

    /*    

    stats:

    HP;
    maxHP;
    DMG;
    speed;
    armor;


    stats in percent:

    dodge;
    dodgeNegation;
    critChance;
    critDamage;
    stun;
    stunNegation;
*/


    //artifacts
    public static Item rock = new Item("Rock", "Useless", 3, ItemType.Artifact)
    {
        details = "its just a small pebble, doesn't hold much of worth... \nunless you want to fight the king ofc"
    };
    public static Item fish = new Item("Fish", "Useless", 1, ItemType.Artifact)
    {
        details = "its a fish.. i like fish:)"
    };
    public static Item VampireRing = new Item("Vampire Ring", "+3% CritChance", 11, ItemType.Artifact)
    {
        details = "Silver ring rumored to strengthen \nits wearer’s blood using magic.",
        stats = { ["critChance"] = 3 },
    };

    //equipment ------

    //head -----
    public static Item baseballCap = new Item("Baseball Cap", "+1 armor", 5, ItemType.equipment)
    {
        stats = { ["armor"] = 1},
        equipmentType = EquipmentType.head,
        details = "cool Cap.. somehow makes it harder to punch your head off"
    };
    public static Item knightHelmet = new Item("Knight Helmet", "+3 armor, -1 speed",7,ItemType.equipment)
    {
        stats = { ["armor"] = 3, ["speed"] = -1},
        equipmentType = EquipmentType.head,
        details =  "heavy knight helmet.. makes it much harder to cut your head off, \nbut its a little too heavy to move properly in"
    };

    public static Item VampireMask = new Item("Vampire Mask", "+20% stun", 23, ItemType.equipment)
    {
        stats = { ["stun"] = 20 },
        equipmentType = EquipmentType.head,
        details = "Elegant mask that makes all opponents freeze in fear"
    };

    //torso ----
    public static Item constructionVest = new Item("Construction Vest", "+2 armor, -40% dodge",6,ItemType.equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = -40 },
        equipmentType = EquipmentType.torso,
        details = "a bright yellow construction vest... \ndefends you a bit, but its hard to dodge in"
    };
    public static Item CloakofDusk = new Item("Cloak of Dusk", "+1 speed, +40% dodge", 18, ItemType.equipment)
    {
        stats = { ["speed"] = 1, ["dodge"] = 40 },
        equipmentType = EquipmentType.torso,
        details = "Black velvet cloak that blends perfectly with shadows"
    };




    //legs ----
    public static Item camoPants = new Item("Camo Pants", "+20% dodge", 3, ItemType.equipment)
    {
        stats = { ["dodge"] = 20 },
        equipmentType = EquipmentType.legs,
        details = "military grade camo pants.. makes it harder to hit you"
    };


    //feet ----
    public static Item sandals = new Item("Sandals", "+1 speed", 5, ItemType.equipment)
    {
        stats = { ["speed"] = 1 },
        equipmentType = EquipmentType.feet,
        details = "some nice sandals.. decently fine to run in"
    };
    public static Item runningShoes = new Item("Running Shoes", "+3 speed", 8, ItemType.equipment)
    {
        stats = { ["speed"] = 3 },
        equipmentType = EquipmentType.feet,
        details = "some beautiful running shoes.. really nice to run in"
    };

    public static Item NightStalkerGreaves = new Item("Night Stalker Greaves", "+50% dodgeNegation", 17, ItemType.equipment)
    {
        stats = { ["dodgeNegation"] = 50 },
        equipmentType = EquipmentType.feet,
        details = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be"
    };


    //consumables
    public static Item smallHealthPotion = new Item("Small Health Potion", "+20 Health", 2, ItemType.consumable)
    {
        stats = { ["HP"] = 20 },
        duration = 0,
        details = "simple health potion, drink it and you regain some health.. not much but being alive is nice"
    };
    public static Item bigHealthPotion = new Item("Big Health Potion", "+50 Health", 4, ItemType.consumable)
    {
        stats = { ["HP"] = 50},
        duration = 0,
        details = "advanced health potion, drink it and you regain a major amount of health.. great for staying alive"
    };


}


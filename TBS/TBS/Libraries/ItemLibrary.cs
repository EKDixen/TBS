public static class ItemLibrary
{

    /*    

    stats:

    Heal;
    maxHP;
    damage;
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
    public static Item rock = new Item("Rock", "Useless", 3,4, ItemType.Artifact)
    {
        details = "its just a small pebble, doesn't hold much of worth... \nunless you want to fight the king ofc"
    };
    public static Item fish = new Item("Fish", "Useless", 1,3, ItemType.Artifact)
    {
        details = "its a fish.. i like fish:)"
    };
    public static Item VampireRing = new Item("Vampire Ring", "+3% CritChance", 11,1, ItemType.Artifact)
    {
        details = "Silver ring rumored to strengthen \nits wearer’s blood using magic.\n+3% CritChance",
        stats = { ["critChance"] = 3 },
    };
    public static Item SilverfallAmulet = new Item("Silverfall Amulet", "+5 maxHP, +3% critChance", 12,1, ItemType.Artifact)
    {
        stats = { ["maxHP"] = 5, ["critChance"] = 3},
        details = "Old amulet with a tiny silver waterfall engraved\n hums faintly with magic\n+5 HP, +3% critChance"
    };


    //equipment ------

    //head -----
    public static Item baseballCap = new Item("Baseball Cap", "+1 armor", 5,2, ItemType.equipment)
    {
        stats = { ["armor"] = 1},
        equipmentType = EquipmentType.head,
        details = "cool Cap.. somehow makes it harder to punch your head off\n+1 armor"
    };
    public static Item knightHelmet = new Item("Knight Helmet", "+3 armor, -1 speed",7,10,ItemType.equipment)
    {
        stats = { ["armor"] = 3, ["speed"] = -1},
        equipmentType = EquipmentType.head,
        details = "heavy knight helmet.. makes it much harder to cut your head off, \nbut its a little too heavy to move properly in\n+3 armor, -1 speed"
    };

    public static Item VampireMask = new Item("Vampire Mask", "+20% stun", 23,4, ItemType.equipment)
    {
        stats = { ["stun"] = 20 },
        equipmentType = EquipmentType.head,
        details = "Elegant mask that makes all opponents freeze in fear \n+20% stun"
    };

    public static Item FallenGuardHelmet = new Item("Fallen Guard's Helmet", "+5 maxHP, +2 Armor...", 14,9, ItemType.equipment)
    {
        stats = { ["maxHP"] = 5, ["armor"] = 2, ["stunNegation"] = 1 },
        equipmentType = EquipmentType.head,
        details = "Cracked iron helmet of a long dead town guard, \n+5 HP, +2 Armor, +1 StunNegation"
    };
    public static Item TatteredHat = new Item("Tattered Hat", "+1 armor, +1 dodge", 5,3, ItemType.equipment)
    {
        stats = { ["armor"] = 1, ["dodge"] = 1 },
        equipmentType = EquipmentType.head,
        details = "A weather-worn hat from a long-forgotten villager \n+1 armor, +1 dodge"
    };
    //torso ----
    public static Item constructionVest = new Item("Construction Vest", "+2 armor, -40% dodge",6,5,ItemType.equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = -40 },
        equipmentType = EquipmentType.torso,
        details = "a bright yellow construction vest... \ndefends you a bit, but its hard to dodge in \n+2 armor, -40% dodge"
    };
    public static Item CloakofDusk = new Item("Cloak of Dusk", "+1 speed, +40% dodge", 35,6, ItemType.equipment)
    {
        stats = { ["speed"] = 1, ["dodge"] = 40 },
        equipmentType = EquipmentType.torso,
        details = "Black velvet cloak that blends perfectly with shadows \n+2 armor, -40% dodge"
    };




    //legs ----
    public static Item camoPants = new Item("Camo Pants", "+20% dodge", 3,2, ItemType.equipment)
    {
        stats = { ["dodge"] = 20 },
        equipmentType = EquipmentType.legs,
        details = "military grade camo pants..\n makes it harder to hit you \n+20% dodge"
    };


    //feet ----
    public static Item sandals = new Item("Sandals", "+1 speed", 5,1, ItemType.equipment)
    {
        stats = { ["speed"] = 1 },
        equipmentType = EquipmentType.feet,
        details = "some nice sandals.. decently fine to run in \n+1 speed"
    };
    public static Item runningShoes = new Item("Running Shoes", "+3 speed", 8,2, ItemType.equipment)
    {
        stats = { ["speed"] = 3 },
        equipmentType = EquipmentType.feet,
        details = "some beautiful running shoes.. really nice to run in \n+3 speed"
    };

    public static Item NightStalkerGreaves = new Item("Night Stalker Greaves", "+50% dodgeNegation", 17,4, ItemType.equipment)
    {
        stats = { ["dodgeNegation"] = 50 },
        equipmentType = EquipmentType.feet,
        details = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be \n+50% dodgeNegation",
        
    };


    //consumables
    public static Item smallHealthPotion = new Item("Small Health Potion", "+20 Health", 2,2, ItemType.consumable)
    {
        stats = { ["heal"] = 20 },
        duration = 0,
        details = "simple health potion, drink it and you regain some health..\n not much but being alive is nice"
    };
    public static Item bigHealthPotion = new Item("Big Health Potion", "+50 Health", 4,3, ItemType.consumable)
    {
        stats = { ["heal"] = 50},
        duration = 0,
        details = "advanced health potion, drink it and you regain\n a major amount of health.. great for staying alive\n+50 Health"
    };

    public static Item speedPotion = new Item("Speed Potion", "+10 Speed", 4, 3, ItemType.consumable)
    {
        duration = 2,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("speed", 10, 2, "self")
        },
        details = "speed potion, drink it to become faster for a bit\n+10 Speed for 2 turns"
    };

}


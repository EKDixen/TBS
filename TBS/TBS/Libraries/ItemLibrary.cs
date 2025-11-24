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


    #region material
    public static Item rock = new Item("Rock", "A common crafting material.", 3, 0, ItemType.Material)
    {
        detailsLore = "Its just a small pebble. Useful for basic crafting, or throwing at kings.",
    };
    public static Item iron = new Item("Iron", "Raw iron ore used for basic forging.", 3, 0, ItemType.Material)
    {
        detailsLore = "Some nice iron ore. Useful for crafting weapons and armor.",
    };
    public static Item fish = new Item("Fish", "A fresh fish, sometimes used in recipes.", 1, 0, ItemType.Material)
    {
        detailsLore = "It's a fish. You like fish.",
    };

    // Monster-specific materials
    public static Item goblinEar = new Item("Goblin Ear", "A trophy from a defeated goblin.", 5, 0, ItemType.Material)
    {
        detailsLore = "Goblins have surprisingly large ears. Useful for certain crafts.",
    };

    public static Item spiderSilk = new Item("Spider Silk", "Strong silk from a forest spider.", 8, 0, ItemType.Material)
    {
        detailsLore = "Incredibly strong and flexible. Prized by armorers.",
    };

    public static Item wolfPelt = new Item("Wolf Pelt", "Thick fur from a dire wolf.", 12, 0, ItemType.Material)
    {
        detailsLore = "Warm and durable. Perfect for crafting protective gear.",
    };

    public static Item frostCore = new Item("Frost Core", "A frozen crystal from an ice wolf.", 20, 0, ItemType.Material)
    {
        detailsLore = "Radiates cold. Used in advanced frost-based crafting.",
    };
    #endregion

    #region craftable equipment (not sold in stores)
    // Head
    public static Item goblinSkullHelm = new Item("Goblin Skull Helm", "+2 armor, +5% dodge", 25, 5, ItemType.equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = 5 },
        equipmentType = EquipmentType.head,
        detailsLore = "A crude helmet fashioned from goblin trophies and reinforced cloth.\nLightweight but surprisingly effective."
    };

    public static Item frostforgedHelm = new Item("Frostforged Helm", "+4 armor, +10 maxHP", 80, 12, ItemType.equipment)
    {
        stats = { ["armor"] = 4, ["maxHP"] = 10 },
        equipmentType = EquipmentType.head,
        detailsLore = "A helmet infused with frost magic. The cold never bothered you anyway."
    };

    // Torso
    public static Item spidersilkVest = new Item("Spidersilk Vest", "+3 armor, +15% dodge", 60, 7, ItemType.equipment)
    {
        stats = { ["armor"] = 3, ["dodge"] = 15 },
        equipmentType = EquipmentType.torso,
        detailsLore = "Woven from spider silk and reinforced with wolf pelt.\nFlexible yet protective."
    };

    public static Item reinforcedIronPlate = new Item("Reinforced Iron Plate", "+6 armor, +15 maxHP, -1 speed", 70, 15, ItemType.equipment)
    {
        stats = { ["armor"] = 6, ["maxHP"] = 15, ["speed"] = -1 },
        equipmentType = EquipmentType.torso,
        detailsLore = "Heavy iron plating reinforced with quality materials.\nProvides excellent protection at the cost of mobility."
    };

    // Legs
    public static Item spidersilkLeggings = new Item("Spidersilk Leggings", "+1 armor, +25% dodge, +1 speed", 55, 4, ItemType.equipment)
    {
        stats = { ["armor"] = 1, ["dodge"] = 25, ["speed"] = 1 },
        equipmentType = EquipmentType.legs,
        detailsLore = "Lightweight leggings woven from spider silk.\nEnhances agility significantly."
    };

    // Feet
    public static Item wolfhideBoots = new Item("Wolfhide Boots", "+2 speed, +2 armor", 40, 3, ItemType.equipment)
    {
        stats = { ["speed"] = 2, ["armor"] = 2 },
        equipmentType = EquipmentType.feet,
        detailsLore = "Sturdy boots lined with wolf pelt.\nProvides both protection and mobility."
    };
    #endregion

    #region artifacts
    public static Item VampireRing = new Item("Vampire Ring", "+3% CritChance", 11,1, ItemType.Artifact)
    {
        detailsLore = "Silver ring rumored to strengthen \nits wearer’s blood using magic.",
        stats = { ["critChance"] = 3 },
    };

    public static Item BasicBackpack = new Item("Basic Backpack", "A simple pack for carrying crafting materials.", 50, 2, ItemType.Backpack)
    {
        stats = { ["materialCapacity"] = 50 },
        detailsLore = "A sturdy canvas backpack. Increases how many materials you can carry without weighing you down."
    };
    public static Item SilverfallAmulet = new Item("Silverfall Amulet", "+5 maxHP, +3% critChance", 12,1, ItemType.Artifact)
    {
        stats = { ["maxHP"] = 5, ["critChance"] = 3},
        detailsLore = "Old amulet with a tiny silver waterfall engraved\n hums faintly with magic" 
    };
    #endregion

    #region equipment
    #endregion

    #region head
    public static Item baseballCap = new Item("Baseball Cap", "+1 armor", 5,2, ItemType.equipment)
    {
        stats = { ["armor"] = 1},
        equipmentType = EquipmentType.head,
        detailsLore = "cool Cap.. somehow makes it harder to punch your head off"
    };
    public static Item knightHelmet = new Item("Knight Helmet", "+3 armor, -1 speed",7,10,ItemType.equipment)
    {
        stats = { ["armor"] = 3, ["speed"] = -1},
        equipmentType = EquipmentType.head,
        detailsLore = "heavy knight helmet.. makes it much harder to cut your head off, \nbut its a little too heavy to move properly in"
    };

    public static Item VampireMask = new Item("Vampire Mask", "+20% stun", 23,4, ItemType.equipment)
    {
        stats = { ["stun"] = 20 },
        equipmentType = EquipmentType.head,
        detailsLore = "Elegant mask that makes all opponents freeze in fear"
    };

    public static Item FallenGuardHelmet = new Item("Fallen Guard's Helmet", "+5 maxHP, +2 Armor...", 14,9, ItemType.equipment)
    {
        stats = { ["maxHP"] = 5, ["armor"] = 2, ["stunNegation"] = 1 },
        equipmentType = EquipmentType.head,
        detailsLore = "Cracked iron helmet of a long dead town guard"
    };
    public static Item TatteredHat = new Item("Tattered Hat", "+1 armor, +1 dodge", 5,3, ItemType.equipment)
    {
        stats = { ["armor"] = 1, ["dodge"] = 1 },
        equipmentType = EquipmentType.head,
        detailsLore = "A weather-worn hat from a long-forgotten villager"
    };
    #endregion

    #region torso
    public static Item constructionVest = new Item("Construction Vest", "+2 armor, -40% dodge",6,5,ItemType.equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = -40 },
        equipmentType = EquipmentType.torso,
        detailsLore = "a bright yellow construction vest... \ndefends you a bit, but its hard to dodge in"
    };
    public static Item CloakofDusk = new Item("Cloak of Dusk", "+1 speed, +40% dodge", 35,6, ItemType.equipment)
    {
        stats = { ["speed"] = 1, ["dodge"] = 40 },
        equipmentType = EquipmentType.torso,
        detailsLore = "Black velvet cloak that blends perfectly with shadows"
    };
    public static Item frozenChestplate = new Item("Frozen chestplate", "+50 max hp, +5 armor", 350, 13, ItemType.equipment)
    {
        stats = { ["maxHP"] = 50, ["armor"] = 5 },
        equipmentType = EquipmentType.torso,
        detailsLore = "Black velvet cloak that blends perfectly with shadows"
    };
    #endregion

    #region legs
    public static Item camoPants = new Item("Camo Pants", "+20% dodge", 3,2, ItemType.equipment)
    {
        stats = { ["dodge"] = 20 },
        equipmentType = EquipmentType.legs,
        detailsLore = "military grade camo pants..\n makes it harder to hit you"
    };
    #endregion

    #region boots
    public static Item sandals = new Item("Sandals", "+1 speed", 5,1, ItemType.equipment)
    {
        stats = { ["speed"] = 1 },
        equipmentType = EquipmentType.feet,
        detailsLore = "some nice sandals.. decently fine to run in"
    };
    public static Item runningShoes = new Item("Running Shoes", "+3 speed", 8,2, ItemType.equipment)
    {
        stats = { ["speed"] = 3 },
        equipmentType = EquipmentType.feet,
        detailsLore = "some beautiful running shoes.. really nice to run in"
    };

    public static Item NightStalkerGreaves = new Item("Night Stalker Greaves", "+50% dodgeNegation", 17,4, ItemType.equipment)
    {
        stats = { ["dodgeNegation"] = 50 },
        equipmentType = EquipmentType.feet,
        detailsLore = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be",
        
    };
    public static Item iceSkates = new Item("Ice Skates", "+5 speed, -2 armor ", 250, 4, ItemType.equipment)
    {
        stats = { ["speed"] = 5, ["armor"] = -2 },
        equipmentType = EquipmentType.feet,
        detailsLore = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be",

    };
    #endregion

    #region consumeables
    public static Item smallHealthPotion = new Item("Small Health Potion", "+20 Health", 2,2, ItemType.consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 20, 0, "self")
        },
        duration = 0,
        detailsLore = "simple health potion, drink it and you regain some health..\n not much but being alive is nice"
    };
    public static Item bigHealthPotion = new Item("Big Health Potion", "+50 Health", 4,3, ItemType.consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 50, 0, "self")
        },
        duration = 0,
        detailsLore = "advanced health potion, drink it and you regain\n a major amount of health.. great for staying alive"
    };

    public static Item speedPotion = new Item("Speed Potion", "+10 Speed", 4, 3, ItemType.consumable)
    {
        duration = 2,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("speed", 10, 2, "self")
        },
        detailsLore = "speed potion, drink it to become faster for a bit"
    };

}

#endregion
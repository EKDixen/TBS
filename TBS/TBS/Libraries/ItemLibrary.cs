public static class ItemLibrary
{

    /*    
      ----------all stats
    HP;
    maxHP;
    DMG;
    speed;
    armor;

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
        details = "its just a small pebble, doesn't hold much of worth... unless you want to fight the king ofc"
    };


    //equipment
    public static Item baseballCap = new Item("Baseball Cap", "+1 armor", 5, ItemType.equipment)
    {
        stats = { ["armor"] = 1},
        equipmentType = EquipmentType.head,
        details = "cool Cap.. somehow makes it harder to punch your head off"
    };
    public static Item sandals = new Item("Sandals", "+1 speed", 5, ItemType.equipment)
    {
        stats = { ["speed"] = 1 },
        equipmentType = EquipmentType.feet,
        details = "some nice sandals.. decently fine to run in"
    };
    public static Item runningShoes = new Item("Running Shoes", "+3 speed", 7, ItemType.equipment)
    {
        stats = { ["speed"] = 3 },
        equipmentType = EquipmentType.feet,
        details = "some beautiful running shoes.. really nice to run in"
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
        stats = { ["HP"] = 20},
        duration = 0,
        details = "advanced health potion, drink it and you regain a major amount of health.. great for staying alive"
    };


}


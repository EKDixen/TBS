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
        details = "cool Cap.. somehow makes it harder to punch your head off"
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


public static class ItemLibrary
{

    /*    
    name;
    level;
    exp;
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
    public static Item rock = new Item("Rock", "Useless", 3, ItemType.equipment)
    {
        details = "its just a small pebble, doesn't hold much of worth... unless you want to fight the king ofc"
    };

    public static Item smallHealthPotion = new Item("Small Health Potion", "+20 Health", 2, ItemType.consumable)
    {
        stats = { ["HP"] = 20 },
        duration = 0,
        details = "simple Health potion, drink it and you regain some health.. not much but being alive is nice"
    };



}


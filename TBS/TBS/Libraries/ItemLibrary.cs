public static class ItemLibrary
{
    public static Item rock = new Item("Rock", "Useless", 1, 3, ItemType.equipment)
    {
        details = "its just a small pebble, doesn't hold much of worth... unless you want to fight the king ofc"
    };
    public static Item lwktestitem2 = new Item("lwk test item 2", "+15 speed", 16, 2, ItemType.equipment)
    {
        stats = { ["speed"] = 15 },
        details = " yeah lwk just a test mate"
    };
    public static Item lwktestitem1 = new Item("lwk test item", "+10 DMG", 16, 2, ItemType.equipment)
    {
        stats = { ["DMG"] = 10 },
        details = "shi this lwk just a test"
    };



}


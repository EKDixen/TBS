public static class EncounterLibrary
{
    public static Encounter FoundCoins = new Encounter(
        "FoundCoins" ,50 ,LocationLibrary.starterTown ,false,
        "You found some coins on the ground."
    );

    public static Encounter LostCoins = new Encounter(
        "LostCoins" ,30 ,LocationLibrary.starterTown ,false,
        "You realize your coins are missing!"
    );

    public static Encounter WildGoblin = new Encounter(
        "WildGoblin",70 ,LocationLibrary.forest ,true,
        "A wild Goblin appears from the bushes!"
    );

    public static Encounter StrangeMushrooms = new Encounter(
        "StrangeMushrooms", 40 ,LocationLibrary.forest ,false,
        "You stumble upon some strange mushrooms."
    );

    public static List<Encounter> AllEncounters = new List<Encounter>()
    {
        FoundCoins,
        LostCoins,
        WildGoblin,
        StrangeMushrooms
    };
}

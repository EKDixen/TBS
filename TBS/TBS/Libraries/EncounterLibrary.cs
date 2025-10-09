public static class EncounterLibrary
{
    public static Encounter FoundCoins = new Encounter(
        "FoundCoins" ,false ,
        "You found some coins on the ground."
    );

    public static Encounter LostCoins = new Encounter(
        "LostCoins" ,false ,
        "You realize your coins are missing!"
    );

    public static Encounter WildGoblin = new Encounter(
        "WildGoblin" ,true ,
        "A wild Goblin appears from the bushes!"
    );

    public static Encounter StrangeMushrooms = new Encounter(
        "StrangeMushrooms" ,false ,
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

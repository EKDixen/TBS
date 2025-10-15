using System.Collections.Generic;

public static class LocationLibrary
{

    public static Location starterTown = new Location(true, "StarterTown", new System.Numerics.Vector2(0, 0), 0, new List<SubLocation>
    {
        new SubLocation("casino", SubLocationType.casino)
        {
            casinoMaxBet = 50
        },
        new SubLocation("bank", SubLocationType.bank)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.smallHealthPotion,2),
                (ItemLibrary.bigHealthPotion,1),
                (ItemLibrary.baseballCap,1),
                (ItemLibrary.sandals,1)

            }
        }
    },
    new Dictionary<Enemy, int>
    {
        { EnemyLibrary.Thug, 80 },
        { EnemyLibrary.Goblin, 20 }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 50},
        { EncounterLibrary.LostCoins, 40}
    });


    public static Location forest = new Location(false, "Forest", new System.Numerics.Vector2(1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.smallHealthPotion,3),
                (ItemLibrary.bigHealthPotion,1),
                (ItemLibrary.runningShoes,1)
            }
        }
    },
    new Dictionary<Enemy, int>
    {
        { EnemyLibrary.VampireSpawn, 100 }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.WildGoblin, 50},
        { EncounterLibrary.StrangeMushrooms, 40},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.LostCoins, 10 }
    });

    public static Location mountain = new Location(false, "Mountain", new System.Numerics.Vector2(-1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.bigHealthPotion,3),
                (ItemLibrary.runningShoes,1)
            }
        }
    },
    new Dictionary<Enemy, int>()
    {

    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 50},
        { EncounterLibrary.LostCoins, 40}
    });

    public static Location lake = new Location(false, "Lake", new System.Numerics.Vector2(0, -1), 0, new List<SubLocation>
    {
        new SubLocation("Bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Enemy, int>()
    {
        
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 50},
        { EncounterLibrary.LostCoins, 40}
    });




    public static List<Location> locations = new List<Location>
    {
        starterTown, forest, mountain, lake
    };

}
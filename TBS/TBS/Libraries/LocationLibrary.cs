using System.Collections.Generic;

public static class LocationLibrary
{

    public static Location starterTown = new Location(true, "StarterTown", new System.Numerics.Vector2(0, 0),0, new List<SubLocation>
    {
        new SubLocation("casino", SubLocationType.casino)
        {
            casinoMaxBet = 50
        },
        new SubLocation("bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Enemy, int>
    {
        { EnemyLibrary.Thug, 80 },
        { EnemyLibrary.Goblin, 20 }
    });


    public static Location forest = new Location(false, "Forest", new System.Numerics.Vector2(1, 0),0, new List<SubLocation>
    {
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.smallHealthPotion,2),

            }
        }
    },
    new Dictionary<Enemy, int>
    {
        { EnemyLibrary.VampireSpawn, 100 }
    });

    public static Location mountain = new Location(false, "Mountain", new System.Numerics.Vector2(-1, 0),0, new List<SubLocation>
    {

    }, new Dictionary<Enemy, int>());

    public static Location lake = new Location(false, "Lake", new System.Numerics.Vector2(0, -1),0, new List<SubLocation>
    {

    }, new Dictionary<Enemy, int>());




    public static List<Location> locations = new List<Location>
    {
        starterTown, forest, mountain, lake
    };

}
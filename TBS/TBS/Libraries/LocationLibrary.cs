using System.Collections.Generic;

public static class LocationLibrary
{

    public static Location starterTown = new Location( "StarterTown", new System.Numerics.Vector2(0, 0), 0, new List<SubLocation>
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
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.LostCoins, 30},
        { EncounterLibrary.BanditAmbush, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.MysteriousShrine, 10}
    });


    public static Location forest = new Location( "Forest", new System.Numerics.Vector2(1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.smallHealthPotion,3),
                (ItemLibrary.bigHealthPotion,1),
                (ItemLibrary.runningShoes,1),
                (ItemLibrary.camoPants,1)
            }
        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.WildGoblin, 35},
        { EncounterLibrary.GoblinPack, 20},
        { EncounterLibrary.VampireAttack, 15},
        { EncounterLibrary.StrangeMushrooms, 30},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.FallenIntoTrap, 15},
        { EncounterLibrary.FoundTreasure, 10}
    });

    public static Location mountain = new Location( "Mountain", new System.Numerics.Vector2(-1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.bigHealthPotion,3),
                (ItemLibrary.runningShoes,1),
                (ItemLibrary.knightHelmet,1),
                (ItemLibrary.constructionVest,1)
            }
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.FoundTreasure, 30},
        { EncounterLibrary.MysteriousShrine, 25},
        { EncounterLibrary.WanderingMerchant, 20}
    });

    public static Location lake = new Location( "Lake", new System.Numerics.Vector2(0, -1), 0, new List<SubLocation>
    {
        new SubLocation("Bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.StrangeMushrooms, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.FallenIntoTrap, 10}
    });




    public static List<Location> locations = new List<Location>
    {
        starterTown, forest, mountain, lake
    };

    public static Dictionary<string, Location> locationMap = locations.ToDictionary(l => l.name);


    public static Location Get(string name)
    {
        locationMap.TryGetValue(name, out var loc);
        return loc;
    }

}
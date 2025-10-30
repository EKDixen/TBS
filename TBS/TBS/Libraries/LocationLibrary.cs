using System.Collections.Generic;

public static class LocationLibrary
{

    public static Location Maplecross = new Location("Maplecross", new System.Numerics.Vector2(0, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
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
        { EncounterLibrary.BanditFight, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.MysteriousShrine, 534565}
    });


    public static Location Greenhollow = new Location("Greenhollow", new System.Numerics.Vector2(1, 0), 0, new List<SubLocation>
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
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.WildGoblin, 35},
        { EncounterLibrary.GoblinPack, 15},
        { EncounterLibrary.StrangeMushrooms, 30},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.FallenIntoTrap, 20},
        { EncounterLibrary.FoundTreasure, 10},
        { EncounterLibrary.RoadGambling, 10}
    });

    public static Location Ironpeak = new Location("Ironpeak", new System.Numerics.Vector2(-1, 0), 1, new List<SubLocation>
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

        },
        new SubLocation("Casino",SubLocationType.casino)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.FoundTreasure, 30},
        { EncounterLibrary.MysteriousShrine, 25},
        { EncounterLibrary.WanderingMerchant, 20},
        { EncounterLibrary.BanditAmbush, 15}
    });

    public static Location Mistport = new Location("Mistport", new System.Numerics.Vector2(0, -1), 0, new List<SubLocation>
    {
        new SubLocation("pond", SubLocationType.pond)
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

    public static Location MossGate = new Location("MossGate", new System.Numerics.Vector2(2, 0), 0, new List<SubLocation>
    {
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.StrangeMushrooms, 40},
        { EncounterLibrary.FallenIntoTrap, 10},
        { EncounterLibrary.GoblinPack, 15},
        { EncounterLibrary.RoadGambling, 10}
    });

    public static Location Nightreach = new Location("Nightreach", new System.Numerics.Vector2(-1, 1), 0, new List<SubLocation>
    {
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.VampireRing,1),
                (ItemLibrary.CloakofDusk,1),
                (ItemLibrary.NightStalkerGreaves,1),
                (ItemLibrary.VampireMask,1)
            }
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundTreasure, 40},
        { EncounterLibrary.VampireAttack, 60}
    });

    public static Location SilverfallRuins = new Location("Silverfall Ruins", new System.Numerics.Vector2(-2, 1), 0, new List<SubLocation>
    {
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<(Item,int)>
            {
                (ItemLibrary.FallenGuardHelmet,1),
                (ItemLibrary.SilverfallAmulet,2)
            }
        }
    },
new Dictionary<Encounter, int>
{
        { EncounterLibrary.FoundTreasure, 20},
        { EncounterLibrary.GhostlyApparition, 60},
        { EncounterLibrary.SkeletonWarriors, 60}
});


    public static List<Location> locations = new List<Location>
    {
        Maplecross, Greenhollow, Ironpeak, Mistport
    };

    public static Dictionary<string, Location> locationMap = locations.ToDictionary(l => l.name);


    public static Location Get(string name)
    {
        locationMap.TryGetValue(name, out var loc);
        return loc;
    }

}
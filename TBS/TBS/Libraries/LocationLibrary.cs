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
            shopItems = new List<Item>
            {
                (ItemLibrary.smallHealthPotion),
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.baseballCap),
                (ItemLibrary.sandals)
            }
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.LostCoins, 30},
        { EncounterLibrary.BanditFight, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.MysteriousShrine, 5}
    });


    public static Location Greenhollow = new Location("Greenhollow", new System.Numerics.Vector2(1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.smallHealthPotion),
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.runningShoes),
                (ItemLibrary.camoPants)
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
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.runningShoes),
                (ItemLibrary.knightHelmet),
                (ItemLibrary.constructionVest)
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
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        },
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
        { EncounterLibrary.FallenIntoTrap, 10},
        { EncounterLibrary.FallingFish, 10}
    });

    public static Location MossGate = new Location("MossGate", new System.Numerics.Vector2(2, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
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
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.VampireRing),
                (ItemLibrary.CloakofDusk),
                (ItemLibrary.NightStalkerGreaves),
                (ItemLibrary.VampireMask)
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
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.FallenGuardHelmet),
                (ItemLibrary.SilverfallAmulet)
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
        Maplecross, Greenhollow, Ironpeak, Mistport,MossGate,Nightreach,SilverfallRuins
    };

    public static Dictionary<string, Location> locationMap = locations.ToDictionary(l => l.name);


    public static Location Get(string name)
    {
        locationMap.TryGetValue(name, out var loc);
        return loc;
    }

}
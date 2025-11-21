using System.Collections.Generic;

public static class LocationLibrary
{
    #region "Coastal Alliance"
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
                (ItemLibrary.sandals),
                (ItemLibrary.speedPotion)
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
    }, "Coastal Alliance");

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

        },
        new SubLocation("Port", SubLocationType.port)
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
   }, "Coastal Alliance");

    public static Location SaltmarshShore = new Location("Saltmarsh Shore", new System.Numerics.Vector2(-1, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FallingFish, 20},
        { EncounterLibrary.SeaBanditRaid, 15},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Coastal Alliance");

    #endregion

    #region "Greenwood Territories"
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
        { EncounterLibrary.GoblinPack, 5},
        { EncounterLibrary.StrangeMushrooms, 30},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.FallenIntoTrap, 20},
        { EncounterLibrary.FoundTreasure, 10},
        { EncounterLibrary.RoadGambling, 10},
        { EncounterLibrary.LearnFirstAid, 5}
    }, "Greenwood Territories");

    public static Location WhisperWood = new Location("WhisperWood", new System.Numerics.Vector2(2, 1), 1, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.ForestSpiderNest, 30},
        { EncounterLibrary.DireWolfHunt, 15},
        { EncounterLibrary.Wolfsensei, 15},
        { EncounterLibrary.DireWolfPack, 10},
        { EncounterLibrary.StrangeMushrooms, 20},
        { EncounterLibrary.FoundTreasure, 15}
    }, "Greenwood Territories");

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
        { EncounterLibrary.RoadGambling, 10},
        { EncounterLibrary.LearnFirstAid, 15}
    }, "Greenwood Territories");

    #endregion

    #region "Fallen Kingdom"
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
            casinoMaxBet = 20
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.FoundTreasure, 30},
        { EncounterLibrary.MysteriousShrine, 25},
        { EncounterLibrary.WanderingMerchant, 20},
        { EncounterLibrary.BanditAmbush, 15}
    }, "Fallen Kingdom");

    public static Location ShattershoreCliffs = new Location("Shattershore Cliffs", new System.Numerics.Vector2(-2, 0), 1, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.KingdomGuardPatrol, 30},
        { EncounterLibrary.SmugglerAmbush, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.BanditAmbush, 15},
        { EncounterLibrary.FallenIntoTrap, 10}
    }, "Fallen Kingdom");

    public static Location WitheredRuins = new Location("Withered Ruins", new System.Numerics.Vector2(-2, 2), 2, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.RuinedGolemAwakens, 25},
        { EncounterLibrary.CorruptedKnightBattle, 25},
        { EncounterLibrary.GhostlyApparition, 30},
        { EncounterLibrary.SkeletonWarriors, 30},
        { EncounterLibrary.FoundTreasure, 20}
    }, "Fallen Kingdom");

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
    }, "Fallen Kingdom");

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
}, "Fallen Kingdom");
    #endregion

    #region "wilderness"
    public static Location FrozenWastes = new Location("Frozen Wastes", new System.Numerics.Vector2(-3, 1), 2, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 40},
        { EncounterLibrary.FrostTrollAmbush, 30},
        { EncounterLibrary.IceMageEncounter, 25},
        { EncounterLibrary.FrozenHorde, 15}
    }, null);

    public static Location TundraMarch = new Location("Tundra March", new System.Numerics.Vector2(-4, 1), 2, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 45},
        { EncounterLibrary.FrostTrollAmbush, 35},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.SnowWraithAttack, 10}
    }, null);
    #endregion

    #region"Frostborn Dominion"
    public static Location SnowfallRidge = new Location("Snowfall Ridge", new System.Numerics.Vector2(-5, 1), 2, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 30},
        { EncounterLibrary.SnowWraithAttack, 25},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.FoundTreasure, 20},
        { EncounterLibrary.MysteriousShrine, 10}
    }, "Frostborn Dominion");

    public static Location EternalIcefall = new Location("Eternal Icefall", new System.Numerics.Vector2(-6, 1), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.SnowWraithAttack, 35},
        { EncounterLibrary.FrozenHorde, 30},
        { EncounterLibrary.FrostTrollAmbush, 25},
        { EncounterLibrary.FoundTreasure, 30},
        { EncounterLibrary.MysteriousShrine, 10}
    }, "Frostborn Dominion");

    public static Location FrostfangCrag = new Location("Frostfang Crag", new System.Numerics.Vector2(-6, 0), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.SnowWraithAttack, 40},
        { EncounterLibrary.FrostTrollAmbush, 30},
        { EncounterLibrary.IceWolfPack, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FallenIntoTrap, 10}
    }, "Frostborn Dominion");

    public static Location Everwinter = new Location("Everwinter", new System.Numerics.Vector2(-5, 0), 3, new List<SubLocation>
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
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.knightHelmet)
            }
        },
        new SubLocation("Casino", SubLocationType.casino)
        {
            casinoMaxBet = 50,
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 30},
        { EncounterLibrary.IceMageEncounter, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FoundCoins, 20},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Frostborn Dominion");

    public static Location IceboundPort = new Location("Icebound Port", new System.Numerics.Vector2(-5, -1), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.runningShoes)
            }
        },
        new SubLocation("Port", SubLocationType.port)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 25},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.FoundCoins, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FallingFish, 10},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Frostborn Dominion");
    #endregion

    public static List<Location> locations = new List<Location>
    {
        Maplecross, Greenhollow, Ironpeak, Mistport, MossGate, Nightreach, SilverfallRuins,
        WhisperWood, SaltmarshShore, ShattershoreCliffs, WitheredRuins, FrozenWastes,
        TundraMarch, SnowfallRidge, EternalIcefall, FrostfangCrag, Everwinter, IceboundPort
    };

    public static Dictionary<string, Location> locationMap = locations.ToDictionary(l => l.name);


    public static Location Get(string name)
    {
        locationMap.TryGetValue(name, out var loc);
        return loc;
    }

}
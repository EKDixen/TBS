using System.Collections.Generic;

public enum CraftingStationType
{
    Forge,
    // Later: Enchanter
}

public class MaterialCost
{
    public Item Material { get; set; }
    public int Quantity { get; set; }

    public MaterialCost(Item material, int quantity)
    {
        Material = material;
        Quantity = quantity;
    }
}

public class Recipe
{
    public string Id { get; set; }
    public string Name { get; set; }
    public CraftingStationType Station { get; set; }
    public Item OutputItem { get; set; }
    public int OutputQuantity { get; set; } = 1;
    public List<MaterialCost> Materials { get; set; } = new List<MaterialCost>();
    public int MoneyCost { get; set; } = 0;
    public string AreaTag { get; set; } // e.g. "Coastal Alliance", "Frostborn Dominion"
}
#region Coastal Alliance Recipes
public static class RecipeLibrary
{
    public static readonly Recipe StarterIronHelm = new Recipe
    {
        Id = "starter_iron_helm",
        Name = "Iron Helm",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.knightHelmet,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.rock, 1)
        },
        MoneyCost = 10,
        AreaTag = "Coastal Alliance"
    };

    public static readonly Recipe GoblinSkullHelm = new Recipe
    {
        Id = "goblin_skull_helm",
        Name = "Goblin Skull Helm",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.goblinSkullHelm,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.goblinEar, 5),
            new MaterialCost(ItemLibrary.rock, 3)
        },
        MoneyCost = 10,
        AreaTag = "Coastal Alliance"
    };

    public static readonly Recipe ReinforcedIronPlate = new Recipe
    {
        Id = "reinforced_iron_plate",
        Name = "Reinforced Iron Plate",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.reinforcedIronPlate,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.iron, 8),
            new MaterialCost(ItemLibrary.rock, 4)
        },
        MoneyCost = 20,
        AreaTag = "Coastal Alliance"
    };
    #endregion

    #region Greenwood Territories Recipes
    public static readonly Recipe WolfhideBoots = new Recipe
    {
        Id = "wolfhide_boots",
        Name = "Wolfhide Boots",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.wolfhideBoots,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.wolfPelt, 2),
            new MaterialCost(ItemLibrary.spiderSilk, 1)
        },
        MoneyCost = 12,
        AreaTag = "Greenwood Territories"
    };

    public static readonly Recipe SpidersilkLeggings = new Recipe
    {
        Id = "spidersilk_leggings",
        Name = "Spidersilk Leggings",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.spidersilkLeggings,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.spiderSilk, 6),
            new MaterialCost(ItemLibrary.wolfPelt, 1)
        },
        MoneyCost = 18,
        AreaTag = "Greenwood Territories"
    };

    public static readonly Recipe SpidersilkVest = new Recipe
    {
        Id = "spidersilk_vest",
        Name = "Spidersilk Vest",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.spidersilkVest,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.spiderSilk, 5),
            new MaterialCost(ItemLibrary.wolfPelt, 3)
        },
        MoneyCost = 20,
        AreaTag = "Greenwood Territories"
    };
    #endregion

    #region Fallen Kingdom Recipes
    public static readonly Recipe IronWarhammerRecipe = new Recipe
    {
        Id = "iron_warhammer",
        Name = "Iron Warhammer",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.IronWarhammer,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.iron, 50),
            new MaterialCost(ItemLibrary.rock, 5)
        },
        MoneyCost = 40, 
        AreaTag = "Fallen Kingdom"
    };
    public static readonly Recipe SpineBreakerRecipe = new Recipe
    {
        Id = "spine_breaker",
        Name = "Spine Breaker",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.SpineBreaker,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.bone, 8),
            new MaterialCost(ItemLibrary.spiderSilk, 2)
        },
        MoneyCost = 30,
        AreaTag = "Fallen Kingdom"
    };
    #endregion

    #region Frostborn Dominion Recipes

    public static readonly Recipe FrostforgedHelm = new Recipe
    {
        Id = "frostforged_helm",
        Name = "Frostforged Helm",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.frostforgedHelm,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.frostCore, 3),
            new MaterialCost(ItemLibrary.iron, 6),
            new MaterialCost(ItemLibrary.wolfPelt, 2)
        },
        MoneyCost = 30,
        AreaTag = "Frostborn Dominion"
    }; 
    public static readonly Recipe GlacierReaverRecipe = new Recipe
    {
        Id = "glacier_reaver",
        Name = "Glacier Reaver",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.GlacierReaver, 
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.iron, 20),
            new MaterialCost(ItemLibrary.frostCore, 5)
        },
        MoneyCost = 150,
        AreaTag = "Frostborn Dominion"
    };
    public static readonly Recipe WinterfangGauntletRecipe = new Recipe
    {
        Id = "winterfang_gauntlet",
        Name = "Winterfang Gauntlet",
        Station = CraftingStationType.Forge,
        OutputItem = ItemLibrary.WinterfangGauntlet,
        OutputQuantity = 1,
        Materials = new List<MaterialCost>
        {
            new MaterialCost(ItemLibrary.iron, 10),     
            new MaterialCost(ItemLibrary.spiderSilk, 7),
            new MaterialCost(ItemLibrary.frostCore, 10)  
        },
        MoneyCost = 100,
        AreaTag = "Frostborn Dominion"
    };
    public static readonly List<Recipe> AllRecipes = new List<Recipe>
    {
        StarterIronHelm,
        GoblinSkullHelm,
        ReinforcedIronPlate,
        WolfhideBoots,
        SpidersilkLeggings,
        SpidersilkVest,
        FrostforgedHelm, 
        IronWarhammerRecipe,
        GlacierReaverRecipe,
        WinterfangGauntletRecipe,
        SpineBreakerRecipe
    };

    public static IEnumerable<Recipe> GetRecipesFor(string areaTag, CraftingStationType station)
    {
        foreach (var r in AllRecipes)
        {
            if (r.Station == station && (r.AreaTag == null || r.AreaTag == areaTag))
                yield return r;
        }
    }
    #endregion
}
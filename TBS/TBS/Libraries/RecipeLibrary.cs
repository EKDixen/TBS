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

    public static readonly List<Recipe> AllRecipes = new List<Recipe>
    {
        StarterIronHelm
    };

    public static IEnumerable<Recipe> GetRecipesFor(string areaTag, CraftingStationType station)
    {
        foreach (var r in AllRecipes)
        {
            if (r.Station == station && (r.AreaTag == null || r.AreaTag == areaTag))
                yield return r;
        }
    }
}
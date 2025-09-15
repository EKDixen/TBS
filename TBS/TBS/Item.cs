using System;
public enum ItemType
{
    consumable,
    equipment
}
public class Item
{

    public string name { get; set; }
    public string description { get; set; }
    public int amount { get; set; }

    public ItemType type { get; set; }

    // Flexible stats
    public Dictionary<string, int> stats { get; set; } = new();

    public Item(string name, string description,int amount, ItemType type)
    {
        this.name = name;
        this.description = description;
        this.amount = amount;
        this.type = type;
    }
    

}


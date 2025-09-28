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

    public int value { get; set; }

    public ItemType type { get; set; }

    // Flexible stats
    public Dictionary<string, int> stats { get; set; } = new();

    public string details { get; set; }

    public Item(string name, string description,int amount,int value ,ItemType type)
    {
        this.name = name;
        this.description = description;
        this.amount = amount;
        this.value = value;
        this.type = type;
    }
    

}


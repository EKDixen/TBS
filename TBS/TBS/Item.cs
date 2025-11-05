using System;
public enum ItemType
{
    consumable,
    equipment,
    Artifact
}
public enum EquipmentType
{
    head,
    torso,
    legs,
    feet
}

public class Item
{

    public string name { get; set; }
    public string description { get; set; }

    public int amount = 1;

    public float weight { get; set; }

    public int value { get; set; }

    public ItemType type { get; set; }
    public EquipmentType equipmentType;
    public int duration;

    // Flexible stats
    public Dictionary<string, int> stats { get; set; } = new();

    public string details { get; set; }

    public Item() { }

    public Item(string name, string description,int value, int weight, ItemType type)
    {
        this.name = name;
        this.description = description;
        this.value = value;
        this.type = type;
        this.weight = weight;
    }
    public Item(Item template)
    {
        this.name = template.name;
        this.description = template.description;
        this.value = template.value;
        this.type = template.type;
        this.equipmentType = template.equipmentType;
        this.duration = template.duration;
        // This is important! We create a new Dictionary
        // so we don't accidentally change the library's stats
        this.stats = new Dictionary<string, int>(template.stats);
        this.details = template.details;
        this.amount = 1; // Default amount for a new item is 1
        this.weight = template.weight;
    }

}


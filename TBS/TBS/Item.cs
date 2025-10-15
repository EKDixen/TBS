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

    public int value { get; set; }

    public ItemType type { get; set; }
    public EquipmentType equipmentType;
    public int duration;

    // Flexible stats
    public Dictionary<string, int> stats { get; set; } = new();

    public string details { get; set; }

    public Item(string name, string description,int value ,ItemType type)
    {
        this.name = name;
        this.description = description;
        this.value = value;
        this.type = type;
    }
    

}


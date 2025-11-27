using System;
using static System.Net.Mime.MediaTypeNames;
public enum ItemType
{
    Consumable,
    Equipment,
    Artifact,
    Material
}
public enum EquipmentType
{
    Head,
    Torso,
    Legs,
    Feet,
    Weapon
}

public class Item
{

    public string name { get; set; }
    public string description { get; set; }

    public int amount = 1;

    public int weight { get; set; }

    public int value { get; set; }

    public ItemType type { get; set; }
    public EquipmentType equipmentType;

    public Attack weaponAttack { get; set; }

    public int duration;

    // Flexible stats
    public Dictionary<string, int> stats { get; set; } = new();
    public List<AttackEffect> effects;

    public string detailsLore { get; set; }

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
        if (template.stats != null) this.stats = new Dictionary<string, int>(template.stats);
        if (template.effects != null) this.effects = new List<AttackEffect>(template.effects);
        this.detailsLore = template.detailsLore;
        this.amount = 1; // Default amount for a new item is 1
        this.weight = template.weight;
        this.weaponAttack = template.weaponAttack;
    }
    public string GetDescription()
    {
        List<string> parts = new List<string>();
        if (type == ItemType.Equipment && equipmentType != EquipmentType.Weapon || type == ItemType.Artifact)
        {
            foreach (var stat in stats)
            {
                string desc = stat.Key switch
                {
                    // Stats
                    "maxHP" => $"Increases max HP by {stat.Value}\n",
                    "armor" => $"Increases armor by {stat.Value}\n",
                    "speed" => $"Increases speed by {stat.Value}\n",

                    // Percent Stats
                    "critChance" => $"Increases crit chance by {stat.Value}%\n",
                    "critDamage" => $"Increases crit damage by {stat.Value}%\n",
                    "dodge" => $"Increases dodge by {stat.Value}%\n",
                    "dodgeNegation" => $"Increases dodge resistance by {stat.Value}%\n",
                    "stun" => $"Increases stun chance by {stat.Value}%\n",
                    "stunNegation" => $"Increases stun resistance by {stat.Value}%\n",

                    // Default fallback
                    _ => $"{stat.Key} {stat.Value}"
                };

                parts.Add(desc);
            }
        }
        else if (type == ItemType.Consumable)
        {
            foreach (var effect in effects)
            {
                string desc = effect.type switch
                {
                    "damage" => $"Deal {effect.value} damage to yourself {(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "heal" => $"Heal yourself for {effect.value} HP {(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "armor" => $"Increase your armor by {effect.value}{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "critChance" => $"Increase your crit chance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "critDamage" => $"Increase your crit damage by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "dodge" => $"Increase your dodge by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "dodgeNegation" => $"Increase your dodge resistance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "stun" => $"Increase your stun chance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "stunNegation" => $"Increase your stun resistance by {effect.value}%{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    "speed" => $"Increase your speed by {effect.value}{(effect.duration > 0 ? $" for {effect.duration} turns\n" : "\n")}",
                    _ => $"{effect.type} {effect.value} {(effect.duration > 0 ? $"({effect.duration} turns)" : "")}"
                };

                parts.Add(desc);
            }
        }
        else if (type == ItemType.Equipment && equipmentType == EquipmentType.Weapon) 
        {
            parts.Add($"gives you the move {weaponAttack.name} as long as its equipped\n");
            parts.Add($"{weaponAttack.GetDescription()}\n");
        }
        parts.Add($"Weighs {weight}\n");
        parts.Add(detailsLore);

        return string.Join("", parts);
    }

}


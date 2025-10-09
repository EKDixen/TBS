using System.Diagnostics;

public class Encounter
{
    public string Name { get; }
    public int Chance { get; }
    public Location Location { get; }
    public bool IsEnemyEncounter { get; }
    public string Description { get; }

    public Encounter(string name, int chance, Location location, bool isEnemyEncounter, string description)
    {
        Name = name;
        Chance = chance;
        Location = location;
        IsEnemyEncounter = isEnemyEncounter;
        Description = description;
    }
}
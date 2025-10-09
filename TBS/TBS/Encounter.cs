using Game.Class;
using System.Diagnostics;

public class Encounter
{
    public string Name;
    public bool IsEnemyEncounter;
    public string Description;

    public Encounter(string name, bool isEnemyEncounter, string description)
    {
        Name = name;
        IsEnemyEncounter = isEnemyEncounter;
        Description = description;
    }
    public Encounter() { }

    private static Random rng = new Random();

    public static List<Encounter> StartTravelEncounters(Location a, Location b)
    {
        Dictionary<Encounter, int> possibleEncounters = GetPossibleEncounters(a, b);

        int encounterThreshold = 1000;
        int encounterRoll;

        int total = possibleEncounters.Values.Sum();
        var picks = new List<Encounter>();

        for (encounterRoll = rng.Next(0, 1001); encounterThreshold > encounterRoll; encounterThreshold -= Program.player.luck)
        {
            int r = rng.Next(0, total);
            int accum = 0;
                
            foreach (var kv in possibleEncounters)
            {
                accum += kv.Value;
                if (r < accum)
                {
                    picks.Add(kv.Key);
                    break;
                }
            }
        }
        return picks;
    }

    public static Dictionary<Encounter, int> GetPossibleEncounters(Location a, Location b)
    {
        var combined = new Dictionary<Encounter, int>();
        void addAll(Dictionary<Encounter, int> src)
        {
            if (src == null) return;
            foreach (var kv in src)
            {
                if (combined.ContainsKey(kv.Key)) combined[kv.Key] += kv.Value;
                else combined[kv.Key] = kv.Value;
            }
        }
        addAll(a.possibleEncounters);
        addAll(b.possibleEncounters);

        if (combined.Count == 0)
        {
            Console.WriteLine("No encounters in these zones.");
            return null;
        }

        return combined;
    }
}
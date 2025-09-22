using System.Diagnostics;

public class Encounter
{
    int encounterChance;

    Location location;
    bool enemyEncounter;
    string name;
    int chance;
    List<Encounter> encounterLibrary;

    public Encounter (string Tname, int Tchance, Location Tlocation, bool TenemyEncounter)
    {
        name = Tname;
        chance = Tchance;
        location = Tlocation;
        enemyEncounter = TenemyEncounter;
    }

    public static void TravelEncounter(int TencounterChance, Location Tlocation)
    {
        Random RND = new Random();


        for (TencounterChance = 100; 0 < TencounterChance; TencounterChance -= RND.Next(15, 51))
        {
            Console.WriteLine("You found an encounter");
        }
        Console.WriteLine($"You get to {Tlocation.name}");
    }
    
    private List<Encounter> CheckEncounters(Location Tlocation)
    {
        List<Encounter> TpossibleEncounters = new List<Encounter>();

        for (int i = 0; i < encounterLibrary.Count; i++)
        {
            if (encounterLibrary[i].location == Tlocation)
            {
                TpossibleEncounters.Add(encounterLibrary[i]);
            }
        }
        return TpossibleEncounters;
    }
}
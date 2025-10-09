using Game.Class;

public static class EncounterManager
{
    private static Random rng = new Random();

    public static List<Encounter> GetTravelEncounters(Location location)
    {
        List<Encounter> result = new List<Encounter>();
        bool continueEncountering = true;

        // Get player luck from your global player system
        // Adjust this line to match your actual code
        int playerLuck = Program.player.luck; // Example — replace as needed

        while (continueEncountering)
        {
            var encounter = GetRandomEncounter(location);
            if (encounter != null)
                result.Add(encounter);

            // Luck check after each encounter
            int threshold = 1000 - playerLuck;
            int roll = rng.Next(0, 1001);

            if (roll > threshold)
            {
                // Player was lucky enough to avoid further encounters
                continueEncountering = false;
            }
        }

        return result;
    }

    public static Encounter GetRandomEncounter(Location location)
    {
        var validEncounters = EncounterLibrary.AllEncounters
            .Where(e => e.Location == location)
            .ToList();

        if (validEncounters.Count == 0)
            return null;

        int totalChance = validEncounters.Sum(e => e.Chance);
        int roll = rng.Next(0, totalChance);

        int cumulative = 0;
        foreach (var e in validEncounters)
        {
            cumulative += e.Chance;
            if (roll < cumulative)
                return e;
        }

        return null;
    }
}
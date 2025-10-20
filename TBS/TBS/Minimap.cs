using Game.Class;

public static class Minimap
{
    public static void DisplayMinimap()
    {
        List<int?> travelLocations = new List<int?> { null, null, null, null, null, null, null, null, null };

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location)
                travelLocations[2] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location)
                travelLocations[8] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location)
                travelLocations[6] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location)
                travelLocations[4] = i;

            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 1) == LocationLibrary.locations[i].location)
                travelLocations[3] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, -1) == LocationLibrary.locations[i].location)
                travelLocations[7] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, -1) == LocationLibrary.locations[i].location)
                travelLocations[9] = i;
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 1) == LocationLibrary.locations[i].location)
                travelLocations[1] = i;
        }

        Console.WriteLine(
            $"{SafeLocationName(travelLocations, 1)} - {SafeLocationName(travelLocations, 2)} - {SafeLocationName(travelLocations, 3)}"
        );
        Console.WriteLine(
            $"{SafeLocationName(travelLocations, 4)} - current - {SafeLocationName(travelLocations, 6)}"
        );
        Console.WriteLine(
            $"{SafeLocationName(travelLocations, 7)} - {SafeLocationName(travelLocations, 8)} - {SafeLocationName(travelLocations, 9)}"
        );

        static string SafeLocationName(List<int?> travelLocations, int travelIndex)
        {
            // Check if travelLocations has this
            if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                return "...";

            int? locIndex = travelLocations[travelIndex];

            // Check if the location is valid
            if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                return "...";

            var location = LocationLibrary.locations[locIndex.Value];

            // If the location exists but is not known
            if (!location.known)
                return "???";

            // If location exists and is known, return its name
            return location?.name ?? "...";
        }



    }

}


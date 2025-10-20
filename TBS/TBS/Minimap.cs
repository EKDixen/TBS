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

        string Cell(string s, int maxLength = 10)
        {
            if (s.Length > maxLength)
                return s.Substring(0, maxLength - 2) + ".."; 
            return s;
        }

        Console.WriteLine(
            $"{Cell(SafeLocationName(travelLocations, 1)),-10} | {Cell(SafeLocationName(travelLocations, 2)),-10} | {Cell(SafeLocationName(travelLocations, 3)),-10}"
        );
        Console.WriteLine(new string('-', 34));
        Console.WriteLine(
            $"{Cell(SafeLocationName(travelLocations, 4)),-10} | {"current",-10} | {Cell(SafeLocationName(travelLocations, 6)),-10}"
        );
        Console.WriteLine(new string('-', 34));
        Console.WriteLine(
            $"{Cell(SafeLocationName(travelLocations, 7)),-10} | {Cell(SafeLocationName(travelLocations, 8)),-10} | {Cell(SafeLocationName(travelLocations, 9)),-10}"
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


using Game.Class;

public static class Minimap
{
    public static void DisplayMinimap(int startX,int startY, int maxContentWidth)
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


        int cellWidth = (maxContentWidth - 2) / 3; // -2 for the two separators "|"

        string Cell(string s, int maxLength)
        {
            if (s.Length > maxLength)
                return s.Substring(0, maxLength - 2) + "..";
            return s.PadRight(maxLength); // Use PadRight to ensure fixed width
        }

        int currentY = startY;

        // --- LINE 1 ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(
            $"{Cell(SafeLocationName(travelLocations, 1), cellWidth)} | " +
            $"{Cell(SafeLocationName(travelLocations, 2), cellWidth)} | " +
            $"{Cell(SafeLocationName(travelLocations, 3), cellWidth)}"
        );
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth));
        currentY++;

        // --- LINE 2 (Current Location) ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(
            $"{Cell(SafeLocationName(travelLocations, 4), cellWidth)} | " +
            $"{Cell("current", cellWidth)} | " +
            $"{Cell(SafeLocationName(travelLocations, 6), cellWidth)}"
        );
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth));
        currentY++;

        // --- LINE 3 ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(
            $"{Cell(SafeLocationName(travelLocations, 7), cellWidth)} | " +
            $"{Cell(SafeLocationName(travelLocations, 8), cellWidth)} | " +
            $"{Cell(SafeLocationName(travelLocations, 9), cellWidth)}"
        );
        currentY++;

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


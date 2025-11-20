using Game.Class;
using System;

public static class Minimap
{
    // Kingdom color mapping
    private static Dictionary<string, ConsoleColor> kingdomColors = new Dictionary<string, ConsoleColor>
    {
        { "Coastal Alliance", ConsoleColor.Cyan },
        { "Greenwood Territories", ConsoleColor.Green },
        { "Fallen Kingdom", ConsoleColor.DarkRed },
        { "Frostborn Dominion", ConsoleColor.Blue }
    };

    public static void DisplayMinimap(int startX,int startY, int maxContentWidth)
    {
        List<int?> travelLocations = new List<int?> { null, null, null, null, null, null, null, null, null, null };

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location)
                travelLocations[2] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location)
                travelLocations[8] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location)
                travelLocations[6] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location)
                travelLocations[4] = i;

            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 1) == LocationLibrary.locations[i].location)
                travelLocations[3] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, -1) == LocationLibrary.locations[i].location)
                travelLocations[7] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, -1) == LocationLibrary.locations[i].location)
                travelLocations[9] = i;
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 1) == LocationLibrary.locations[i].location)
                travelLocations[1] = i;
        }


        int cellWidth = (maxContentWidth -3) / 3; // -2 for the two separators "|"

        string Cell(string s, int maxLength)
        {
            if (s.Length > maxLength)
                return s.Substring(0, maxLength - 2) + "..";
            return s.PadRight(maxLength); // Use PadRight to ensure fixed width
        }

        int currentY = startY;

        // --- LINE 1 ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 1, cellWidth, startX, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 2, cellWidth, startX + cellWidth + 3, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 3, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth+2));
        currentY++;

        // --- LINE 2 (Current Location) ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 4, cellWidth, startX, currentY);
        Console.Write(" | ");
        Console.SetCursorPosition(startX + cellWidth + 3, currentY);
        WriteCurrentLocationColored(cellWidth);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 6, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        // --- SEPARATOR ---
        Console.SetCursorPosition(startX, currentY);
        Console.Write(new string('-', maxContentWidth+2));
        currentY++;

        // --- LINE 3 ---
        Console.SetCursorPosition(startX, currentY);
        WriteColoredCell(travelLocations, 7, cellWidth, startX, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 8, cellWidth, startX + cellWidth + 3, currentY);
        Console.Write(" | ");
        WriteColoredCell(travelLocations, 9, cellWidth, startX + (cellWidth + 3) * 2, currentY);
        currentY++;

        void WriteColoredCell(List<int?> travelLocations, int travelIndex, int cellWidth, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            
            string locationName = SafeLocationName(travelLocations, travelIndex);
            
            // Don't color ??? or ... locations
            if (locationName == "???" || locationName == "...")
            {
                Console.Write(Cell(locationName, cellWidth));
                return;
            }
            
            string kingdom = GetLocationKingdom(travelLocations, travelIndex);
            
            // Get color for this kingdom
            ConsoleColor color = ConsoleColor.Gray; // Default color
            if (kingdom != null && kingdomColors.ContainsKey(kingdom))
            {
                color = kingdomColors[kingdom];
            }
            
            // Write with color
            Console.ForegroundColor = color;
            Console.Write(Cell(locationName, cellWidth));
            Console.ResetColor();
        }

        void WriteCurrentLocationColored(int cellWidth)
        {
            var currentLocation = LocationLibrary.Get(Program.player.currentLocation);
            string kingdom = currentLocation?.kingdom;
            
            ConsoleColor color = ConsoleColor.Gray; // Default for no kingdom
            if (kingdom != null && kingdomColors.ContainsKey(kingdom))
            {
                color = kingdomColors[kingdom];
            }
            
            Console.ForegroundColor = color;
            Console.Write(Cell("current", cellWidth));
            Console.ResetColor();
        }

        static string GetLocationKingdom(List<int?> travelLocations, int travelIndex)
        {
            if (travelIndex < 0 || travelIndex >= travelLocations.Count)
                return null;

            int? locIndex = travelLocations[travelIndex];

            if (locIndex == null || locIndex < 0 || locIndex >= LocationLibrary.locations.Count)
                return null;

            var location = LocationLibrary.locations[locIndex.Value];
            return location?.kingdom;
        }

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
            if (!Program.player.knownLocationnames.Contains(location.name))
                return "???";

            // If location exists and is known, return its name
            return location?.name ?? "...";
        }



    }

}


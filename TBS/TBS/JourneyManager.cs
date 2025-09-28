

using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class JourneyManager
{

    

    /*Location forest = new Location(false, "Forest",new System.Numerics.Vector2(1, 0));  
    Location mountain = new Location(false, "Mountain",new System.Numerics.Vector2(-1, 0));
    Location lake = new Location(false, "Lake",new System.Numerics.Vector2(0, -1));*/

    public void ChoseTravelDestination()
    {
        Console.WriteLine("\nwhere do you wish to travel (type out the number next to it)");
        Console.WriteLine("Locations you currently know: ");

        for (int i = 0; i < Program.player.knownLocations.Count; i++)  
        {
            if (Program.player.knownLocations[i] != Program.player.currentLocation) Console.WriteLine(Program.player.knownLocations[i].name + " : " + (i + 1));
            else Console.WriteLine(Program.player.knownLocations[i].name + " : " + (i + 1)+" (current location)");
        }
        Console.WriteLine("\nor explore for a new location : 0");


        int targetDes;
        if (int.TryParse(Console.ReadLine(), out targetDes)) 
        {
            Console.WriteLine("");
            if (targetDes == 0) Explore();
            else if(targetDes <= Program.player.knownLocations.Count) Travel(Program.player.knownLocations[targetDes-1]);
            else
            {
                Console.WriteLine("--------dude you dont know any location with that number------- \n");
                ChoseTravelDestination();
                return;
            }
        }
        else
        {
            Console.WriteLine("--------look mate you gotta type a number----------- \n");
            ChoseTravelDestination();
            return;
        }

    }
    public void Travel(Location TtargetDis)
    {
        Console.WriteLine("\ngoing to " + TtargetDis.name);
        Program.player.currentLocation = TtargetDis;
        //Program.db.SavePlayer(Program.player);
        Program.SavePlayer();

        Encounter.TravelEncounter(100 ,TtargetDis);
        Program.MainMenu(); //remove when encounters are done
    }
    public void Explore()
    {
        /*List<Location> explorableLocations= new List<Location>();

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location 
                && !LocationLibrary.locations[i].known) explorableLocations.Add(LocationLibrary.locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location 
                && !LocationLibrary.locations[i].known) explorableLocations.Add(LocationLibrary.locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location 
                && !LocationLibrary.locations[i].known) explorableLocations.Add(LocationLibrary.locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location 
                && !LocationLibrary.locations[i].known) explorableLocations.Add(LocationLibrary.locations[i]);
        }
        if (explorableLocations.Count != 0)
        {
            Random rand = new Random();
            int randomDir = rand.Next(0, explorableLocations.Count);

            Encounter.TravelEncounter(100, explorableLocations[randomDir]);


            Program.player.knownLocations.Add(explorableLocations[randomDir]);
            Program.player.currentLocation = explorableLocations[randomDir];
            LocationLibrary.locations[randomDir].known = true;

            Program.SavePlayer();
        }*/
        List<int> explorableLocations = new List<int>();

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location
                && !LocationLibrary.locations[i].known) explorableLocations.Add(i);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location
                && !LocationLibrary.locations[i].known) explorableLocations.Add(i);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location
                && !LocationLibrary.locations[i].known) explorableLocations.Add(i);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location
                && !LocationLibrary.locations[i].known) explorableLocations.Add(i);
        }
        if (explorableLocations.Count != 0)
        {
            Random rand = new Random();
            int randomDir = rand.Next(0, explorableLocations.Count);

            Encounter.TravelEncounter(100, LocationLibrary.locations[explorableLocations[randomDir]]);

            Program.player.knownLocations.Add(LocationLibrary.locations[explorableLocations[randomDir]]);
            Program.player.currentLocation = LocationLibrary.locations[explorableLocations[randomDir]];
            LocationLibrary.locations[explorableLocations[randomDir]].known = true;

            Program.SavePlayer();
        }
        else
        {
            Console.WriteLine("\ncant explore from here");
        }
        Program.MainMenu(); //remove when encounters are done
    }
    public void StartEncounter()
    {

    }
}
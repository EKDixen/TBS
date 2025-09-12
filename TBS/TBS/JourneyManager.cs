

using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class JourneyManager
{
    public List<Location> locations = new List<Location>();

    Location starterTown = new Location(true,"StarterTown",new System.Numerics.Vector2(0,0));
    Location forest = new Location(false, "Forest",new System.Numerics.Vector2(1, 0));  
    Location mountain = new Location(false, "Mountain",new System.Numerics.Vector2(-1, 0));
    Location lake = new Location(false, "Lake",new System.Numerics.Vector2(0, -1));


    public void AddLocations()
    {
        locations.Add(starterTown);

        Program.player.knownLocations.Add(starterTown);
        Program.player.currentLocation = starterTown;
        Console.WriteLine(Program.player.currentLocation.name);
        
        locations.Add(forest);
        locations.Add(mountain);
        locations.Add(lake);
    }
    public void ChoseTravelDestination()
    {
        Console.WriteLine("Locations you currently know: ");
        for (int i = 0; i < Program.player.knownLocations.Count; i++)  
        { 
            Console.WriteLine(Program.player.knownLocations[i].name+" : " + (i+1));
        }
        Console.WriteLine("\nwhich one do you wish to travel to? (type out the number next to it)");
        Console.WriteLine("Or do you wish to explore for a new location? ( if so type 0 )");
        

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
        Program.db.SavePlayer(Program.player);
    }
    public void Explore()
    {
        List<Location> explorableLocations= new List<Location>();

        for (int i = 0; i < locations.Count; i++)
        {
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, 1) == locations[i].location && !locations[i].known) explorableLocations.Add(locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, -1) == locations[i].location && !locations[i].known) explorableLocations.Add(locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 0) == locations[i].location && !locations[i].known) explorableLocations.Add(locations[i]);
            if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 0) == locations[i].location && !locations[i].known) explorableLocations.Add(locations[i]);
        }
        if (explorableLocations.Count != 0)
        {
            Random rand = new Random();
            int randomDir = rand.Next(0, explorableLocations.Count);
            
            Console.WriteLine("\nexploring: " + explorableLocations[randomDir].name);
            Program.player.knownLocations.Add(explorableLocations[randomDir]);
            Program.db.SavePlayer(Program.player);
        }
        else
        {
            Console.WriteLine("\ncant explore from here");
        }

    }
    public void StartEncounter()
    {

    }
}
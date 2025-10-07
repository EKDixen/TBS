

using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class JourneyManager
{
    
   
    public void ChoseTravelDestination()
    {
        Console.WriteLine($"Would you like to purchase a carriage ride to your destination : 1 \nor travel by foot (has the option of exploring) : 2");
        int.TryParse(Console.ReadLine(), out int ride);
        if (ride == null || ride > 2 || ride < 1)
        {
            Console.WriteLine("--------look mate you gotta type a number----------- \n");
            ChoseTravelDestination();
            return;
        }
        else if (ride == 1)
        {
            Console.WriteLine("\nwhere do you wish to travel (type out the number next to it)");
            Console.WriteLine("Locations you currently know: ");

            for (int i = 0; i < Program.player.knownLocations.Count; i++)
            {
                if (Program.player.knownLocations[i] != Program.player.currentLocation) 
                {
                    float price = (Program.player.currentLocation.location - Program.player.knownLocations[i].location).Length() * 2 + 
                        Program.player.currentLocation.travelPrize + Program.player.knownLocations[i].travelPrize;
                    Console.WriteLine($"{Program.player.knownLocations[i].name}  :  {(i + 1)}  (price: {price})"); 
                    
                }
                else Console.WriteLine(Program.player.knownLocations[i].name + " : " + (i + 1) + " (current location)");
            }
            Console.WriteLine("or go back : 0");

            int targetDes;
            if (int.TryParse(Console.ReadLine(), out targetDes))
            {

                Console.WriteLine("");
                if (targetDes == 0) { Console.Clear(); Program.MainMenu(); return; }
                else if (targetDes <= Program.player.knownLocations.Count && targetDes >= 0 && Program.player.knownLocations[targetDes - 1] != Program.player.currentLocation) 
                {
                    float price = (Program.player.currentLocation.location - Program.player.knownLocations[targetDes - 1].location).Length() * 2 +
                        Program.player.currentLocation.travelPrize + Program.player.knownLocations[targetDes - 1].travelPrize;

                    if (Program.player.money >= price)
                    {
                        Program.player.money -= (int)Math.Floor(price);
                        Travel(Program.player.knownLocations[targetDes - 1], true);
                    }
                    else
                    {
                        Console.WriteLine("--------you dont have enough money for that------- \n");
                        ChoseTravelDestination();
                        return;
                    }

                
                }
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
        else if (ride == 2)
        {
            Console.WriteLine("\nwhere do you wish to travel (type out the number next to it)");
            Console.WriteLine("Locations you can currently travel to by foot (only adjacent locations): ");

            List<int> travelLocations = new List<int>();

            for (int i = 0; i < LocationLibrary.locations.Count; i++)
            {
                if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location
                    && LocationLibrary.locations[i].known) travelLocations.Add(i);
                if (Program.player.currentLocation.location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location
                    && LocationLibrary.locations[i].known) travelLocations.Add(i);
                if (Program.player.currentLocation.location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location
                    && LocationLibrary.locations[i].known) travelLocations.Add(i);
                if (Program.player.currentLocation.location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location
                    && LocationLibrary.locations[i].known) travelLocations.Add(i);
            }

            for (int i = 0; i < travelLocations.Count; i++)
            {
                Console.WriteLine($"{LocationLibrary.locations[travelLocations[i]].name} : {i+1}");
            }
            Console.WriteLine("\nor explore for a new location : 0");
            Console.WriteLine("or go back : -1");

            int targetDes;
            if (int.TryParse(Console.ReadLine(), out targetDes))
            {
                Console.WriteLine("");
                if (targetDes == 0) Explore();
                else if (targetDes <= travelLocations.Count && targetDes >= 0 && LocationLibrary.locations[travelLocations[targetDes - 1]] != Program.player.currentLocation) Travel(LocationLibrary.locations[travelLocations[targetDes - 1]],false);
                else if (targetDes == -1) { Console.Clear(); Program.MainMenu(); return; }
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
    }
    public void Travel(Location TtargetDis, bool cartin)
    {
        Console.WriteLine("\ngoing to " + TtargetDis.name);
        Program.player.currentLocation = TtargetDis;
        Program.db.SavePlayer(Program.player);
        Program.SavePlayer();
        if(cartin != true)
        {
            Encounter.TravelEncounter(100, TtargetDis);
        }
        Program.MainMenu(); 
    }
    public void Explore()
    {
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
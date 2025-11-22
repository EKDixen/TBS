

using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class JourneyManager
{
    
   
    public void ChoseTravelDestination()
    {
        MainUI.ClearMainArea();

        MainUI.WriteInMainArea($"Would you like to purchase a carriage ride to your destination : 1");
        MainUI.WriteInMainArea("or travel by foot (has the option of exploring) : 2");
        int.TryParse(Console.ReadKey().KeyChar.ToString(), out int ride);
        if (ride == null || ride > 2 || ride < 1)
        {
            MainUI.WriteInMainArea("--------look mate you gotta type a number----------- \n");
            ChoseTravelDestination();
            return;
        }
        else if (ride == 1)
        {
            MainUI.WriteInMainArea("\nwhere do you wish to travel (type out the number next to it)");
            MainUI.WriteInMainArea("Locations you currently know: ");

            for (int i = 0; i < Program.player.knownLocationnames.Count; i++)
            {
                if (LocationLibrary.Get(Program.player.knownLocationnames[i]).kingdom == LocationLibrary.Get(Program.player.currentLocation).kingdom)
                {
                    if (LocationLibrary.Get(Program.player.knownLocationnames[i]) != LocationLibrary.Get(Program.player.currentLocation))
                    {
                        float price = (LocationLibrary.Get(Program.player.currentLocation).location - LocationLibrary.Get(Program.player.knownLocationnames[i]).location).Length() * 2 +
                            LocationLibrary.Get(Program.player.currentLocation).travelPrice + LocationLibrary.Get(Program.player.knownLocationnames[i]).travelPrice;
                        MainUI.WriteInMainArea($"{LocationLibrary.Get(Program.player.knownLocationnames[i]).name}  :  {(i + 1)}  (price: {(int)Math.Floor(price)})");

                    }
                    else MainUI.WriteInMainArea(LocationLibrary.Get(Program.player.knownLocationnames[i]).name + " : " + (i + 1) + " (current location)");
                }
            }
            MainUI.WriteInMainArea("or go back : 0");

            int targetDes;
            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out targetDes))
            {

                MainUI.WriteInMainArea("");
                if (targetDes == 0) { MainUI.ClearMainArea(); ; Program.MainMenu(); return; }
                else if (targetDes <= Program.player.knownLocationnames.Count && targetDes >= 0 && LocationLibrary.Get(Program.player.knownLocationnames[targetDes - 1]) != LocationLibrary.Get(Program.player.currentLocation)) 
                {
                    float price = (LocationLibrary.Get(Program.player.currentLocation).location - LocationLibrary.Get(Program.player.knownLocationnames[targetDes - 1]).location).Length() * 2 +
                        LocationLibrary.Get(Program.player.currentLocation).travelPrice + LocationLibrary.Get(Program.player.knownLocationnames[targetDes - 1]).travelPrice;

                    if (Program.player.money >= price)
                    {
                        Program.player.money -= (int)Math.Floor(price);
                        Travel(LocationLibrary.Get(Program.player.knownLocationnames[targetDes - 1]), true);
                    }
                    else
                    {
                        MainUI.WriteInMainArea("--------you dont have enough Rai for that------- \n");
                        ChoseTravelDestination();
                        return;
                    }

                
                }
                else
                {
                    MainUI.WriteInMainArea("--------dude you dont know any location with that number------- \n");
                    ChoseTravelDestination();
                    return;
                }
            }
            else
            {
                MainUI.WriteInMainArea("--------look mate you gotta type a number----------- \n");
                ChoseTravelDestination();
                return;
            }
        }
        else if (ride == 2)
        {
            MainUI.WriteInMainArea("\nwhere do you wish to travel (type out the number next to it)");
            MainUI.WriteInMainArea("Locations you can currently travel to by foot \n(only adjacent locations): ");

            List<int> travelLocations = new List<int>();

            for (int i = 0; i < LocationLibrary.locations.Count; i++)
            {

                if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location
                    && Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) travelLocations.Add(i);
                if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location
                    && Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) travelLocations.Add(i);
                if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location
                    && Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) travelLocations.Add(i);
                if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location
                    && Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) travelLocations.Add(i);
            }
                MainUI.WriteInMainArea("");
            for (int i = 0; i < travelLocations.Count; i++)
            {
                MainUI.WriteInMainArea($"{LocationLibrary.locations[travelLocations[i]].name} : {i+2}");
            }
            MainUI.WriteInMainArea("explore for a NEW location : 1");
            MainUI.WriteInMainArea("return to main menu : 0");

            int targetDes;
            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out targetDes))
            {
                MainUI.WriteInMainArea("");
                if (targetDes == 1) Explore();
                else if (targetDes == 0) { MainUI.ClearMainArea(); ; Program.MainMenu(); return; }
                else if (targetDes <= travelLocations.Count+1 && targetDes > 0 && LocationLibrary.locations[travelLocations[targetDes - 2]] != LocationLibrary.Get(Program.player.currentLocation)) Travel(LocationLibrary.locations[travelLocations[targetDes - 2]],false);
                else
                {
                    MainUI.WriteInMainArea("--------dude you dont know any location with that number------- \n");
                    ChoseTravelDestination();
                    return;
                }
            }
            else
            {
                MainUI.WriteInMainArea("--------look mate you gotta type a number----------- \n");
                ChoseTravelDestination();
                return;
            }
        }
    }
    public void Travel(Location TtargetDis, bool cartin)
    {
        MainUI.WriteInMainArea("\nTraveling to " + TtargetDis.name + "...");
        
        Location previousLocation = LocationLibrary.Get(Program.player.currentLocation);
        
        if(cartin != true)
        {
            EncounterManager encounterManager = new EncounterManager(Program.player);
            encounterManager.ProcessTravelEncounters(previousLocation, TtargetDis, false);
            
            // Only update location if player didn't flee back
            if (!CombatManager.playerFledLastCombat)
            {
                Program.player.currentLocation = TtargetDis.name;
                Program.db.SavePlayer(Program.player);
                Program.SavePlayer();
            }
        }
        else
        {
            Program.player.currentLocation = TtargetDis.name;
            Program.db.SavePlayer(Program.player);
            Program.SavePlayer();
            MainUI.WriteInMainArea("You arrive safely via carriage.");
            MainUI.WriteInMainArea("Press Enter to continue...");
            Console.ReadLine();
        }
        
        //Minimap.DisplayMinimap();
        Program.MainMenu(); 
    }
    public void Explore()
    {
        List<int> explorableLocations = new List<int>();

        for (int i = 0; i < LocationLibrary.locations.Count; i++)
        {
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, 1) == LocationLibrary.locations[i].location
                && !Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) explorableLocations.Add(i);
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(0, -1) == LocationLibrary.locations[i].location
                && !Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) explorableLocations.Add(i);
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(1, 0) == LocationLibrary.locations[i].location
                && !Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) explorableLocations.Add(i);
            if (LocationLibrary.Get(Program.player.currentLocation).location + new System.Numerics.Vector2(-1, 0) == LocationLibrary.locations[i].location
                && !Program.player.knownLocationnames.Contains(LocationLibrary.locations[i].name)) explorableLocations.Add(i);
        }
        if (explorableLocations.Count != 0)
        {
            Random rand = new Random();
            int randomDir = rand.Next(0, explorableLocations.Count);
            Location newLocation = LocationLibrary.locations[explorableLocations[randomDir]];
            Location previousLocation = LocationLibrary.Get(Program.player.currentLocation);

            MainUI.WriteInMainArea($"\nExploring towards {newLocation.name}...");
            
            EncounterManager encounterManager = new EncounterManager(Program.player);
            encounterManager.ProcessTravelEncounters(previousLocation, newLocation, true);

            // Only update location if player didn't flee back
            if (!CombatManager.playerFledLastCombat)
            {
                MainUI.WriteInMainArea($"\nYou discovered {newLocation.name}!");
                Program.player.knownLocationnames.Add(newLocation.name);
                Program.player.currentLocation = newLocation.name;
                Program.SavePlayer();
            }
        }
        else
        {
            MainUI.WriteInMainArea("\nThere are no unexplored locations adjacent to your current location.");
            MainUI.WriteInMainArea("\nPress Enter to continue...");
            Console.ReadLine();
        }
        //Minimap.DisplayMinimap();
        Program.MainMenu(); //remove when encounters are done
    }
    public void StartEncounter()
    {

    }
}

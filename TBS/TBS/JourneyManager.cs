

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
        int.TryParse(Console.ReadLine(), out int ride);
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

            for (int i = 0; i < Program.player.knownLocations.Count; i++)
            {
                if (Program.player.knownLocations[i] != Program.player.currentLocation) 
                {
                    float price = (Program.player.currentLocation.location - Program.player.knownLocations[i].location).Length() * 2 + 
                        Program.player.currentLocation.travelPrice + Program.player.knownLocations[i].travelPrice;
                    MainUI.WriteInMainArea($"{Program.player.knownLocations[i].name}  :  {(i + 1)}  (price: {price})"); 
                    
                }
                else MainUI.WriteInMainArea(Program.player.knownLocations[i].name + " : " + (i + 1) + " (current location)");
            }
            MainUI.WriteInMainArea("or go back : 0");

            int targetDes;
            if (int.TryParse(Console.ReadLine(), out targetDes))
            {

                MainUI.WriteInMainArea("");
                if (targetDes == 0) { MainUI.ClearMainArea(); ; Program.MainMenu(); return; }
                else if (targetDes <= Program.player.knownLocations.Count && targetDes >= 0 && Program.player.knownLocations[targetDes - 1] != Program.player.currentLocation) 
                {
                    float price = (Program.player.currentLocation.location - Program.player.knownLocations[targetDes - 1].location).Length() * 2 +
                        Program.player.currentLocation.travelPrice + Program.player.knownLocations[targetDes - 1].travelPrice;

                    if (Program.player.money >= price)
                    {
                        Program.player.money -= (int)Math.Floor(price);
                        Travel(Program.player.knownLocations[targetDes - 1], true);
                    }
                    else
                    {
                        MainUI.WriteInMainArea("--------you dont have enough money for that------- \n");
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
                MainUI.WriteInMainArea($"\n{LocationLibrary.locations[travelLocations[i]].name} : {i+1}");
            }
            MainUI.WriteInMainArea("\nor explore for a new location : 0");
            MainUI.WriteInMainArea("or go back : -1");

            int targetDes;
            if (int.TryParse(Console.ReadLine(), out targetDes))
            {
                MainUI.WriteInMainArea("");
                if (targetDes == 0) Explore();
                else if (targetDes <= travelLocations.Count && targetDes >= 0 && LocationLibrary.locations[travelLocations[targetDes - 1]] != Program.player.currentLocation) Travel(LocationLibrary.locations[travelLocations[targetDes - 1]],false);
                else if (targetDes == -1) { MainUI.ClearMainArea(); ; Program.MainMenu(); return; }
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
        MainUI.WriteInMainArea("\ngoing to " + TtargetDis.name);
        Program.player.currentLocation = TtargetDis;
        Program.db.SavePlayer(Program.player);
        Program.SavePlayer();
        if(cartin != true)
        {
            List<Encounter> encounters = Encounter.StartTravelEncounters(TtargetDis, Program.player.currentLocation);

            if (encounters.Count == 0)
            {
                MainUI.WriteInMainArea("Your travel was peaceful.");
            }
            else
            {
                foreach (var e in encounters)
                {
                    MainUI.WriteInMainArea(e.Description);
                    if (e.IsEnemyEncounter)
                    {
                        // Start combat
                        MainUI.WriteInMainArea("Start Combat");
                    }
                }
            }
        }
        //Minimap.DisplayMinimap();
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

            List<Encounter> encounters = Encounter.StartTravelEncounters(LocationLibrary.locations[explorableLocations[randomDir]], Program.player.currentLocation);

            if (encounters.Count == 0)
            {
                MainUI.WriteInMainArea("Your journey was peaceful.");
            }
            else
            {
                foreach (var e in encounters)
                {
                    MainUI.WriteInMainArea(e.Description);
                    if (e.IsEnemyEncounter)
                    {
                        // Handle combat here
                    }
                }
            }

            MainUI.WriteInMainArea("\ngoing to "+ LocationLibrary.locations[explorableLocations[randomDir]].name);
            Program.player.knownLocations.Add(LocationLibrary.locations[explorableLocations[randomDir]]);
            Program.player.currentLocation = LocationLibrary.locations[explorableLocations[randomDir]];
            LocationLibrary.locations[explorableLocations[randomDir]].known = true;

            Program.SavePlayer();
        }
        else
        {
            MainUI.WriteInMainArea("\ncant explore from here");
        }
        //Minimap.DisplayMinimap();
        Program.MainMenu(); //remove when encounters are done
    }
    public void StartEncounter()
    {

    }
}


class JourneyManager
{
    public List<Location> locations = new List<Location>();
    public List<Location> knownLocations = new List<Location>();

    Location starterTown = new Location(true,"StarterTown");
    Location forest = new Location(false, "Forest");  
    Location mountain = new Location(false, "Mountain");
    Location lake = new Location(false, "Lake");

    public void AddLocations()
    {
        locations.Add(starterTown);
        knownLocations.Add(starterTown);
        locations.Add(forest);
        locations.Add(mountain);
        locations.Add(lake);
    }
    public void ChoseTravelDestination()
    {
        Console.WriteLine("Locations you currently know: ");
        for (int i = 0; i < knownLocations.Count; i++)  
        { 
            Console.WriteLine(knownLocations[i].name+" : " + (i+1));
        }
        Console.WriteLine("");
        Console.WriteLine("which one do you wish to travel to? (type out the number next to it)");
        Console.WriteLine("Or do you wish to explore for a new location? ( if so type 0 )");
        

        int targetDes;
        if (int.TryParse(Console.ReadLine(), out targetDes)) 
        {
            Console.WriteLine("");
            if (targetDes == 0) Explore();
            else if(targetDes <= knownLocations.Count) Travel(knownLocations[targetDes]);
            else
            {
                Console.WriteLine("you dont know any location with that number");
                ChoseTravelDestination();
                return;
            }
        }

    }
    public void Travel(Location TtargetDis)
    {
        Console.WriteLine("");
        Console.WriteLine("going to "+ TtargetDis.name);
    }
    public void Explore()
    {
        Console.WriteLine("exploring");
    }




}


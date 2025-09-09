

class JourneyManager
{
    public Location currentLocation;
    public List<Location> locations = new List<Location>();
    public List<Location> knownLocations = new List<Location>();

    Location starterTown = new Location(true,"StarterTown",new System.Numerics.Vector2(0,0));
    Location forest = new Location(false, "Forest",new System.Numerics.Vector2(1, 0));  
    Location mountain = new Location(false, "Mountain",new System.Numerics.Vector2(-1, 0));
    Location lake = new Location(false, "Lake",new System.Numerics.Vector2(0, -1));

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
        Console.WriteLine("\nwhich one do you wish to travel to? (type out the number next to it)");
        Console.WriteLine("Or do you wish to explore for a new location? ( if so type 0 )");
        

        int targetDes;
        if (int.TryParse(Console.ReadLine(), out targetDes)) 
        {
            Console.WriteLine("");
            if (targetDes == 0) Explore();
            else if(targetDes <= knownLocations.Count) Travel(knownLocations[targetDes-1]);
            else
            {
                Console.WriteLine("you dont know any location with that number \n");
                ChoseTravelDestination();
                return;
            }
        }
        else
        {
            Console.WriteLine("you gotta type a number \n");
            ChoseTravelDestination();
            return;
        }

    }
    public void Travel(Location TtargetDis)
    {
        Console.WriteLine("\ngoing to " + TtargetDis.name);
        currentLocation = TtargetDis;
    }
    public void Explore()
    {
        Console.WriteLine("\nexploring");
        Random random = new Random();
        int locationFound = random.Next(1,4);
        if (locationFound == 1)
        {
            for (int i = 0; i < knownLocations.Count - 1; i++)
            {
                if (locations[i].location == currentLocation.location + new System.Numerics.Vector2(1, 0) && 
                (knownLocations[i].location == currentLocation.location + new System.Numerics.Vector2(1, 0)))
                {

                }
            }
        }
    }




}


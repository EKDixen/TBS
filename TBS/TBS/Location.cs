using System.Numerics;

public class Location
{
    public string name;
    public bool known;
    public Vector2 location;

    public List<SubLocation> subLocationsHere = new List<SubLocation>();

    public Location() { } // Deserialize
    public Location(bool Tknown, string Tname, Vector2 Tlocation, List<SubLocation> subLocations)
    {
        known = Tknown;
        name = Tname;
        location = Tlocation;
        subLocationsHere = subLocations;
    }

}


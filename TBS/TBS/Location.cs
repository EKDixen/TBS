using System.Collections.Generic;
using System.Numerics;

public class Location
{
    public string name;
    public bool known;
    public Vector2 location;

    public List<SubLocation> subLocationsHere = new List<SubLocation>();
    public Dictionary<Enemy, int> PossibleEncounters { get; set; }

    public Location() { } // Deserialize
    public Location(bool Tknown, string Tname, Vector2 Tlocation, List<SubLocation> subLocations, Dictionary<Enemy, int> TpossibleEncounters = null)
    {
        known = Tknown;
        name = Tname;
        location = Tlocation;
        subLocationsHere = subLocations;
        PossibleEncounters = TpossibleEncounters ?? new Dictionary<Enemy, int>();
    }

}
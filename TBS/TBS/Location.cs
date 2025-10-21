using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;

public class Location
{
    public string name;
    
    public Vector2 location;

    public int travelPrice;

    public List<SubLocation> subLocationsHere = new List<SubLocation>();
    [JsonIgnore]
    public Dictionary<Enemy, int> possibleEnemy { get; set; }
    [JsonIgnore]
    public Dictionary<Encounter, int> possibleEncounters { get; set; }

    public Location() { } // Deserialize
    public Location(string Tname, Vector2 Tlocation, int travelPrize, List<SubLocation> subLocations, Dictionary<Enemy, int> TpossibleEnemy = null, Dictionary<Encounter, int> TpossibleEncounters = null)
    {
        name = Tname;
        location = Tlocation;
        subLocationsHere = subLocations;
        possibleEnemy = TpossibleEnemy ?? new Dictionary<Enemy, int>();
        possibleEncounters = TpossibleEncounters ?? new Dictionary<Encounter, int>();
        this.travelPrice = travelPrize;
    }

}
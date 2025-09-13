using System.Numerics;

public class Location
{
    public string name;
    public bool known;
    public Vector2 location;


    public Location() { } // IK SLET (Brugt til saving)
    public Location(bool Tknown, string Tname, Vector2 Tlocation)
    {
        known = Tknown;
        name = Tname;
        location = Tlocation;
    }

}


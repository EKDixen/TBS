using System.Numerics;

class Location
{
    public string name;
    public bool known;
    public Vector2 location;


    public Location(bool Tknown, string Tname, Vector2 Tlocation)
    {
        known = Tknown;
        name = Tname;
        location = Tlocation;
    }

}


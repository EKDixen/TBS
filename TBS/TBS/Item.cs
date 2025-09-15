using System;
public enum ItemType
{
    consumable,
    equipment
}
public class Item
{

    public string name;
    public string amount;
    public string description;
    public Item itemType;

    public int HP;
    public int DMG;
    public int speed;
    public int armor;

    //Extra stats (regnet i %)
    public int dodge;
    public int dodgeNegation;
    public int critChance;
    public int critDamage;
    public int stun;
    public int stunNegation;

    public int luck;


    public Item(string Tname, string Tamount,string tDescription, Item Ttype, int Thp, int Tdmg, int Tspeed, int Tarmor, 
        int Tdodge, int TdodgeNegation, int TcritChance, int TcritDamage, int Tstun, int TstunNegation, int Tluck)
    {

        name = Tname;
        amount = Tamount;
        description = tDescription;
        itemType = Ttype;
        HP = Thp;
        DMG = Tdmg;
        speed = Tspeed;
        armor = Tarmor;
        dodge = Tdodge;
        dodgeNegation = TdodgeNegation;
        critChance = TcritChance;
        critDamage = TcritDamage;
        stun = Tstun;
        stunNegation = TstunNegation;
        luck = Tluck;
    }

}


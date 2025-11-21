using Game.Class;

public class Class
{
    public string name;

    public static int THP = 0; public static int TmaxHP = 5;
    public static int Tspeed = 1; public static int Tarmor = 1;
    public static int Tdodge = 5; public static int TdodgeNegation = 5; 
    public static int Tcritchance = 5; public static int TcritDamage = 5; 
    public static int Tstun = 5; public static int TstunNegation = 5;

    public Class(){ }//never forget😔
    public Class(string name)
    {
        this.name = name;
    }
    public void LevelupStats()
    {
        Program.player.HP += THP;
        Program.player.maxHP += TmaxHP;
        Program.player.speed += Tspeed;
        Program.player.armor += Tarmor;
        Program.player.dodge += Tdodge;
        Program.player.dodgeNegation += TdodgeNegation;
        Program.player.critChance += Tcritchance;
        Program.player.critDamage += TcritDamage;
        Program.player.stun += Tstun;
        Program.player.stunNegation += TstunNegation;
    }
}


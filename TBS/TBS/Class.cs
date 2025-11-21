using Game.Class;

public class Class
{
    public string name;
    public string description; 
    public bool isMagic;       

    public int THP = 0;
    public int TmaxHP = 5;
    public int Tspeed = 1;
    public int Tarmor = 1;
    public int Tdodge = 5;
    public int TdodgeNegation = 5;
    public int Tcritchance = 5;
    public int TcritDamage = 5;
    public int Tstun = 5;
    public int TstunNegation = 5;
    public int Tluck  = 0; 

    public Class() { }//never forget😔
    public Class(string name)
    {
        this.name = name;
    }

    public void LevelupStats()
    {
        Program.player.HP += this.THP;
        Program.player.maxHP += this.TmaxHP;
        Program.player.speed += this.Tspeed;
        Program.player.armor += this.Tarmor;
        Program.player.dodge += this.Tdodge;
        Program.player.dodgeNegation += this.TdodgeNegation;
        Program.player.critChance += this.Tcritchance;
        Program.player.critDamage += this.TcritDamage;
        Program.player.stun += this.Tstun;
        Program.player.stunNegation += this.TstunNegation;
        Program.player.luck += this.Tluck; 
    }
}
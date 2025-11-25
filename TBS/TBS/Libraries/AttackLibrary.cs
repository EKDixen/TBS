using System.Collections.Generic;

public static class AttackLibrary
{
    #region healing

    public static Attack FirstAid = new Attack("First Aid", new List<AttackEffect> {
        new AttackEffect("heal", 15, 0, "self")
    });

    public static Attack GroupHeal = new Attack("Group heal", new List<AttackEffect> {
        new AttackEffect("heal", 10, 0, "allAllies")
    });

    #endregion

    #region buff

    public static Attack Focus = new Attack("Focus", new List<AttackEffect> {
        new AttackEffect("critChance", 10, 2, "self"),
        new AttackEffect("dodgenegation", 10, 2, "self")
    });

    public static Attack ManUp = new Attack("Man up", new List<AttackEffect> {
        new AttackEffect("armor", 15, 2, "self"),
    }); 
    
    public static Attack GlacialCoating = new Attack("Glacial Coating", new List<AttackEffect> {
        new AttackEffect("speed", -2, 2, "self"),
        new AttackEffect("armor", 5, 3, "self"),
    });

    public static Attack FrostBarrier = new Attack("Frost Barrier", new List<AttackEffect> {
        new AttackEffect("damage", 5, 0, "enemy"),       
        new AttackEffect("armor", 15, 2, "self"),         
        new AttackEffect("dodge", 50, 2, "self"),
        new AttackEffect("speed", -5, 3, "self")
    });

    #endregion

    #region other

    public static Attack ThrowHands = new Attack("Throw hands", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemy")
    });

    public static Attack VampiricSlash = new Attack("Vampiric Slash", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemy"),
        new AttackEffect("heal", 10, 0, "self")
    });

    public static Attack Slash = new Attack("Slash", new List<AttackEffect> {
        new AttackEffect("damage", 10, 0, "enemy")
    });

    public static Attack EtherealTouch = new Attack("EtherealTouch", new List<AttackEffect> {
        new AttackEffect("damage", 15, 0, "enemy"),
        new AttackEffect("dodge", -10, 3, "enemy"),
        new AttackEffect("speed", -1, 5, "enemy")
    });

    public static Attack Snowball = new Attack("Snowball", new List<AttackEffect> {
        new AttackEffect("dodge", -10, 1, "enemy"),
        new AttackEffect("speed", -1, 2, "enemy")
    }); 
    
    public static Attack Bite = new Attack("Bite", new List<AttackEffect> {
        new AttackEffect("damage", 8, 0, "enemy"),
        new AttackEffect("damage", 5, 3, "enemy"),
    }); 
    
    public static Attack Hailstorm = new Attack("Hailstorm", new List<AttackEffect> {
        new AttackEffect("damage", 10, 0, "enemies"),
        new AttackEffect("dodge", -10, 1, "enemies"),
    });

    public static Attack HeavySlam = new Attack("Heavy Slam", new List<AttackEffect> {
        new AttackEffect("damage", 25, 0, "enemy"),      
        new AttackEffect("armor", -5, 2, "enemy")       
    });

    public static Attack VenomStrike = new Attack("Venom Strike", new List<AttackEffect> {   
        new AttackEffect("damage", 5, 5, "enemy")        
    });

    public static Attack RecklessSwing = new Attack("Reckless Swing", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemy"),            
        new AttackEffect("armor", -5, 1, "enemy"),             
        new AttackEffect("armor", -5, 2, "self")               
    });

    public static Attack GlacialSweep = new Attack("Glacial Sweep", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemies"), 
        new AttackEffect("speed", -3, 1, "enemies")  
    });

    public static Attack BoneShatter = new Attack("Bone Shatter", new List<AttackEffect> {
        new AttackEffect("damage", 10, 0, "enemy"),       
        new AttackEffect("stunNegation", -25, 3, "enemy")
    });
    public static Attack FoulThrust = new Attack("Foul Thrust", new List<AttackEffect> {
        new AttackEffect("damage", 15, 0, "enemy"),       
        new AttackEffect("dodge", -15, 1, "enemy")        
    });
    #endregion
















}

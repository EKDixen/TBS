using System.Collections.Generic;

public static class AttackLibrary
{
    #region healing

    public static Attack FirstAid = new Attack("First Aid", new List<AttackEffect> {
        new AttackEffect("heal", 15, 0, "self")
    });

    public static Attack GroupHeal = new Attack("Group heal", new List<AttackEffect> {
        new AttackEffect("heal", 10, 0, "ally")
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

    #endregion
















}

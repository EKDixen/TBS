using System.Collections.Generic;

public static class AttackLibrary
{
    public static Attack ThrowHands = new Attack("Throw hands", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemy")
    });


    public static Attack FirstAid = new Attack("First Aid", new List<AttackEffect> {
        new AttackEffect("heal", 15, 0, "self")
    });

    public static Attack Focus = new Attack("Focus", new List<AttackEffect> {
        new AttackEffect("critChance", 10, 2, "self"),
        new AttackEffect("dodgenegation", 10, 2, "self")
    });

    public static Attack ManUp = new Attack("Man up", new List<AttackEffect> {
        new AttackEffect("armor", 15, 2, "self"),
    });

    public static Attack VampiricSlash = new Attack("Vampiric Slash", new List<AttackEffect> {
        new AttackEffect("damage", 20, 0, "enemy"),
        new AttackEffect("heal", 10, 0, "self")
    });

    public static Attack Slash = new Attack("Slash", new List<AttackEffect> {
        new AttackEffect("damage", 10, 0, "enemy")
    });
}

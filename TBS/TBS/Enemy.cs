public class MaterialDrop
{
    public Item Material { get; set; }
    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }
    public float DropChance { get; set; } // 0.0 to 1.0

    public MaterialDrop(Item material, int minQty, int maxQty, float dropChance)
    {
        Material = material;
        MinQuantity = minQty;
        MaxQuantity = maxQty;
        DropChance = dropChance;
    }
}

public class Enemy : Combatant
{
    public List<Attack> attacks = new List<Attack>();
    public Dictionary<Attack, int> attackWeights = new Dictionary<Attack, int>();
    public List<MaterialDrop> materialDrops = new List<MaterialDrop>();

    public Enemy(string TenemyName, int Tlevel, int Texp, int THP, int Tspeed, int Tarmor,
        int Tdodge, int TdodgeNegation, int Tcritchance, int TcritDamage, int Tstun,
        int TstunNegation, int Tmoney)
    {
        name = TenemyName;
        level = Tlevel;
        exp = Texp;
        HP = THP;
        speed = Tspeed;
        armor = Tarmor;
        dodge = Tdodge;
        dodgeNegation = TdodgeNegation;
        critChance = Tcritchance;
        critDamage = TcritDamage;
        stun = Tstun;
        stunNegation = TstunNegation;
        money = Tmoney;
    }

    public Enemy() { }
    
    public Attack SelectWeightedAttack()
    {
        if (attackWeights.Count == 0 || attacks.Count == 0)
        {
            var rng = new Random();
            return attacks[rng.Next(attacks.Count)];
        }
        
        int totalWeight = 0;
        foreach (var weight in attackWeights.Values)
        {
            totalWeight += weight;
        }
        
        var rng2 = new Random();
        int randomValue = rng2.Next(totalWeight);
        int cumulativeWeight = 0;
        
        foreach (var kvp in attackWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue < cumulativeWeight)
            {
                return kvp.Key;
            }
        }
        
        return attacks[0];
    }
}
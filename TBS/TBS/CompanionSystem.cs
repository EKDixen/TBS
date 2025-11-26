using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// Companion system that stores companions in player's statTracker
/// Supports multiple companions based on class
/// </summary>
public static class CompanionSystem
{
    private const string COMPANION_DATA_KEY = "companionData";
    private const string COMPANION_COUNT_KEY = "companionCount";
    
    /// <summary>
    /// Get maximum number of companions for player
    /// Stored in statTracker as "maxCompanions"
    /// </summary>
    public static int GetMaxCompanions(Player player)
    {
        // Check if max companions is set in statTracker
        if (player.HasStat("maxCompanions"))
        {
            return player.GetStat("maxCompanions");
        }
        
        // If not set, initialize based on class
        int maxCompanions = 1; // Default
        
        if (player.playerClass != null)
        {
            string className = player.playerClass.name.ToLower();
            
            // Necromancer can have 1 companion (special graveyard mechanic)
            if (className.Contains("necromancer")) maxCompanions = 1;
            
            // Beastmaster/Tamer can have 2 companions
            else if (className.Contains("beast") || className.Contains("tamer")) maxCompanions = 2;
            
            // Berserker cannot have allies
            else if (className.Contains("berserker")) maxCompanions = 0;
        }
        
        // Store in statTracker for future use
        player.SetStat("maxCompanions", maxCompanions);
        
        return maxCompanions;
    }
    
    /// <summary>
    /// Set maximum number of companions for player
    /// </summary>
    public static void SetMaxCompanions(Player player, int max)
    {
        player.SetStat("maxCompanions", max);
    }
    
    /// <summary>
    /// Get all companions for a player
    /// </summary>
    public static List<Enemy> GetCompanions(Player player)
    {
        List<Enemy> companions = new List<Enemy>();
        
        int count = player.GetStat(COMPANION_COUNT_KEY);
        if (count <= 0) return companions;
        
        for (int i = 0; i < count; i++)
        {
            string key = $"{COMPANION_DATA_KEY}_{i}";
            
            if (player.HasStat($"{key}_len"))
            {
                try
                {
                    // Get the stored JSON data
                    string jsonData = GetStringFromStatTracker(player, key);
                    
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        var jsonSettings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Auto
                        };
                        
                        var companion = JsonConvert.DeserializeObject<Enemy>(jsonData, jsonSettings);
                        if (companion != null)
                        {
                            // Reconstruct attackWeights from attacks
                            if (companion.attacks != null && companion.attacks.Count > 0)
                            {
                                companion.attackWeights = new Dictionary<Attack, int>();
                                int weightPerAttack = 100 / companion.attacks.Count;
                                foreach (var attack in companion.attacks)
                                {
                                    companion.attackWeights[attack] = weightPerAttack;
                                }
                            }
                            
                            companions.Add(companion);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading companion {i}: {ex.Message}");
                    // Clear corrupted data
                    ClearCompanionData(player, i);
                }
            }
        }
        
        return companions;
    }
    
    /// <summary>
    /// Save companions to player's statTracker
    /// </summary>
    public static void SaveCompanions(Player player, List<Enemy> companions)
    {
        // Clear ALL old companion data thoroughly
        int oldCount = player.GetStat(COMPANION_COUNT_KEY);
        for (int i = 0; i < Math.Max(oldCount, 10); i++) // Clear up to 10 slots to be safe
        {
            ClearCompanionData(player, i);
        }
        
        // Save new companions
        player.SetStat(COMPANION_COUNT_KEY, companions.Count);
        
        // Configure JSON settings to handle circular references and ignore attackWeights
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new IgnoreAttackWeightsResolver()
        };
        
        for (int i = 0; i < companions.Count; i++)
        {
            string key = $"{COMPANION_DATA_KEY}_{i}";
            string jsonData = JsonConvert.SerializeObject(companions[i], jsonSettings);
            SaveStringToStatTracker(player, key, jsonData);
        }
    }
    
    /// <summary>
    /// Add a companion to the player's party
    /// </summary>
    public static bool AddCompanion(Player player, Enemy companion)
    {
        int maxCompanions = GetMaxCompanions(player);
        
        if (maxCompanions == 0)
        {
            Console.WriteLine($"Your class ({player.playerClass.name}) cannot have companions!");
            return false;
        }
        
        var companions = GetCompanions(player);
        
        if (companions.Count >= maxCompanions)
        {
            // Remove oldest companion (first in list)
            if (companions.Count > 0)
            {
                Console.WriteLine($"{companions[0].name} has left your party to make room for {companion.name}.");
                companions.RemoveAt(0);
            }
        }
        
        // Ensure companion has full HP and is marked as ally
        companion.HP = companion.maxHP > 0 ? companion.maxHP : companion.HP;
        companion.IsAlly = true;
        companion.IsPlayer = false;
        
        companions.Add(companion);
        SaveCompanions(player, companions);
        
        Console.WriteLine($"{companion.name} has joined your party!");
        return true;
    }
    
    /// <summary>
    /// Remove a specific companion
    /// </summary>
    public static bool RemoveCompanion(Player player, int index)
    {
        var companions = GetCompanions(player);
        
        if (index < 0 || index >= companions.Count)
        {
            Console.WriteLine("Invalid companion index.");
            return false;
        }
        
        string companionName = companions[index].name;
        companions.RemoveAt(index);
        SaveCompanions(player, companions);
        
        Console.WriteLine($"{companionName} has left your party.");
        return true;
    }
    
    /// <summary>
    /// Remove all dead companions
    /// </summary>
    public static void RemoveDeadCompanions(Player player)
    {
        var companions = GetCompanions(player);
        var aliveCompanions = companions.Where(c => c.IsAlive()).ToList();
        
        if (aliveCompanions.Count < companions.Count)
        {
            int deadCount = companions.Count - aliveCompanions.Count;
            Console.WriteLine($"{deadCount} companion(s) have fallen and left your party.");
            SaveCompanions(player, aliveCompanions);
        }
    }
    
    /// <summary>
    /// Remove all companions (used when changing from Necromancer class)
    /// </summary>
    public static void DismissAllCompanions(Player player, string reason = "")
    {
        var companions = GetCompanions(player);
        
        if (companions.Count > 0)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                Console.WriteLine(reason);
            }
            
            foreach (var companion in companions)
            {
                Console.WriteLine($"{companion.name} has left your party.");
            }
        }
        
        // Clear all companion data completely
        ClearAllCompanionData(player);
    }
    
    /// <summary>
    /// Completely clear all companion data from statTracker
    /// </summary>
    private static void ClearAllCompanionData(Player player)
    {
        // Get old count
        int oldCount = player.GetStat(COMPANION_COUNT_KEY);
        
        // Clear all companion data slots
        for (int i = 0; i < Math.Max(oldCount, 10); i++) // Check up to 10 slots to be safe
        {
            ClearCompanionData(player, i);
        }
        
        // Reset count to 0
        player.SetStat(COMPANION_COUNT_KEY, 0);
    }
    
    /// <summary>
    /// Handle class change - dismiss companions if changing from Necromancer
    /// </summary>
    public static void OnClassChange(Player player, Class oldClass, Class newClass)
    {
        if (oldClass != null && oldClass.name.ToLower().Contains("necromancer"))
        {
            // Dismiss all companions when leaving Necromancer
            DismissAllCompanions(player, "As you abandon the dark arts, the spirits bound to you are released...");
        }
        
        // Update max companions based on new class
        int newMax = 1; // Default
        
        if (newClass != null)
        {
            string className = newClass.name.ToLower();
            
            if (className.Contains("necromancer")) newMax = 1;
            else if (className.Contains("beast") || className.Contains("tamer")) newMax = 2;
            else if (className.Contains("berserker")) newMax = 0;
        }
        
        SetMaxCompanions(player, newMax);
        
        // If new class can't have companions, dismiss all
        if (newMax == 0)
        {
            DismissAllCompanions(player, $"The {newClass.name} walks alone. Your companions depart.");
        }
        // If new max is less than current companions, remove excess
        else
        {
            var companions = GetCompanions(player);
            if (companions.Count > newMax)
            {
                Console.WriteLine($"Your new class can only have {newMax} companion(s).");
                var toKeep = companions.Take(newMax).ToList();
                var toRemove = companions.Skip(newMax).ToList();
                
                foreach (var companion in toRemove)
                {
                    Console.WriteLine($"{companion.name} has left your party.");
                }
                
                SaveCompanions(player, toKeep);
            }
        }
    }
    
    /// <summary>
    /// Heal all companions to full HP
    /// </summary>
    public static void HealAllCompanions(Player player)
    {
        var companions = GetCompanions(player);
        
        if (companions.Count == 0)
        {
            Console.WriteLine("You don't have any companions.");
            return;
        }
        
        foreach (var companion in companions)
        {
            companion.HP = companion.maxHP;
        }
        
        SaveCompanions(player, companions);
        Console.WriteLine("All companions have been healed!");
    }
    
    /// <summary>
    /// Display all companions
    /// </summary>
    public static void DisplayCompanions(Player player)
    {
        var companions = GetCompanions(player);
        
        if (companions.Count == 0)
        {
            Console.WriteLine("You don't have any companions.");
            return;
        }
        
        Console.WriteLine($"\n=== Your Companions ({companions.Count}/{GetMaxCompanions(player)}) ===");
        for (int i = 0; i < companions.Count; i++)
        {
            var c = companions[i];
            string status = c.IsAlive() ? "Alive" : "Dead";
            Console.WriteLine($"{i + 1}. {c.name} - Lvl {c.level} - HP: {c.HP}/{c.maxHP} ({status})");
        }
    }
    
    /// <summary>
    /// Recruit companion by name from EnemyLibrary
    /// </summary>
    public static bool RecruitByName(Player player, string enemyName)
    {
        Enemy enemyTemplate = enemyName.ToLower() switch
        {
            "goblin" => EnemyLibrary.Goblin,
            "thug" => EnemyLibrary.Thug,
            "sea bandit" => EnemyLibrary.SeaBandit,
            "smuggler" => EnemyLibrary.Smuggler,
            "healer" => EnemyLibrary.Healer,
            "forest spider" => EnemyLibrary.ForestSpider,
            "dire wolf" => EnemyLibrary.DireWolf,
            "vampire spawn" => EnemyLibrary.VampireSpawn,
            "ghostly apparition" => EnemyLibrary.GhostlyApparition,
            "skeleton warrior" => EnemyLibrary.SkeletonWarrior,
            "kingdom guard" => EnemyLibrary.KingdomGuard,
            "corrupted knight" => EnemyLibrary.CorruptedKnight,
            "ruined golem" => EnemyLibrary.RuinedGolem,
            "ice wolf" => EnemyLibrary.IceWolf,
            "frost troll" => EnemyLibrary.FrostTroll,
            "glacier golem" => EnemyLibrary.GlacierGolem,
            "ice mage" => EnemyLibrary.IceMage,
            "snow wraith" => EnemyLibrary.SnowWraith,
            "elf hunter" => EnemyLibrary.ElfHunter,
            "shadow lynx" => EnemyLibrary.ShadowLynx,
            "elf mystic" => EnemyLibrary.ElfMystic,
            _ => null
        };

        if (enemyTemplate == null)
        {
            Console.WriteLine($"Unknown enemy: {enemyName}");
            return false;
        }

        return AddCompanion(player, CloneEnemy(enemyTemplate));
    }
    
    /// <summary>
    /// Recruit a dead player as a spirit companion (Necromancer only)
    /// </summary>
    public static bool RecruitSpirit(Player necromancer, Player deadPlayer)
    {
        // Check if player is a Necromancer
        if (necromancer.playerClass == null || !necromancer.playerClass.name.ToLower().Contains("necromancer"))
        {
            Console.WriteLine("Only Necromancers can raise spirits from the dead!");
            return false;
        }
        
        // Check if necromancer is higher level
        if (necromancer.level <= deadPlayer.level)
        {
            Console.WriteLine($"The spirit of {deadPlayer.name} is too powerful for you to control.");
            Console.WriteLine($"You must be higher level than the spirit (Your level: {necromancer.level}, Spirit level: {deadPlayer.level})");
            return false;
        }
        
        // Convert dead player to companion
        var spirit = new Enemy(
            $"{deadPlayer.name}'s Spirit",
            deadPlayer.level,
            0, // No exp
            deadPlayer.maxHP,
            deadPlayer.speed,
            deadPlayer.armor,
            deadPlayer.dodge,
            deadPlayer.dodgeNegation,
            deadPlayer.critChance,
            deadPlayer.critDamage,
            deadPlayer.stun,
            deadPlayer.stunNegation,
            0 // No money
        );
        
        spirit.maxHP = deadPlayer.maxHP;
        spirit.HP = spirit.maxHP;
        
        // Copy attacks from dead player
        if (deadPlayer.equippedAttacks != null && deadPlayer.equippedAttacks.Count > 0)
        {
            spirit.attacks = new List<Attack>();
            spirit.attackWeights = new Dictionary<Attack, int>();
            
            foreach (var attack in deadPlayer.equippedAttacks.Where(a => a != null))
            {
                spirit.attacks.Add(attack);
                spirit.attackWeights[attack] = 100 / deadPlayer.equippedAttacks.Count(a => a != null);
            }
        }
        
        // If no equipped attacks, give basic attack
        if (spirit.attacks == null || spirit.attacks.Count == 0)
        {
            spirit.attacks = new List<Attack> { AttackLibrary.ThrowHands };
            spirit.attackWeights = new Dictionary<Attack, int> { { AttackLibrary.ThrowHands, 100 } };
        }
        
        return AddCompanion(necromancer, spirit);
    }
    
    /// <summary>
    /// Clone an enemy
    /// </summary>
    private static Enemy CloneEnemy(Enemy original)
    {
        var clone = new Enemy(
            original.name,
            original.level,
            original.exp,
            original.HP,
            original.speed,
            original.armor,
            original.dodge,
            original.dodgeNegation,
            original.critChance,
            original.critDamage,
            original.stun,
            original.stunNegation,
            original.money
        );

        clone.maxHP = original.maxHP > 0 ? original.maxHP : original.HP;
        
        if (original.attacks != null)
        {
            clone.attacks = new List<Attack>(original.attacks);
        }
        
        if (original.attackWeights != null)
        {
            clone.attackWeights = new Dictionary<Attack, int>(original.attackWeights);
        }

        if (original.materialDrops != null)
        {
            clone.materialDrops = new List<MaterialDrop>(original.materialDrops);
        }

        return clone;
    }
    
    /// <summary>
    /// Clear companion data for a specific index
    /// </summary>
    private static void ClearCompanionData(Player player, int index)
    {
        string key = $"{COMPANION_DATA_KEY}_{index}";
        
        // Clear length key
        if (player.HasStat($"{key}_len"))
        {
            int length = player.GetStat($"{key}_len");
            player.ResetStat($"{key}_len");
            
            // Clear all character keys
            for (int i = 0; i < length; i++)
            {
                if (player.HasStat($"{key}_c{i}"))
                {
                    player.ResetStat($"{key}_c{i}");
                }
            }
        }
    }
    
    // Helper methods to store strings in statTracker (which only stores ints)
    // We'll use a simple encoding: store each character's ASCII value in separate keys
    
    private static void SaveStringToStatTracker(Player player, string baseKey, string value)
    {
        // Store length
        player.SetStat($"{baseKey}_len", value.Length);
        
        // Store each character as int
        for (int i = 0; i < value.Length; i++)
        {
            player.SetStat($"{baseKey}_c{i}", (int)value[i]);
        }
    }
    
    private static string GetStringFromStatTracker(Player player, string baseKey)
    {
        if (!player.HasStat($"{baseKey}_len")) return null;
        
        int length = player.GetStat($"{baseKey}_len");
        char[] chars = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            if (player.HasStat($"{baseKey}_c{i}"))
            {
                chars[i] = (char)player.GetStat($"{baseKey}_c{i}");
            }
        }
        
        return new string(chars);
    }
}

/// <summary>
/// Custom JSON contract resolver to ignore attackWeights during serialization
/// </summary>
public class IgnoreAttackWeightsResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
{
    protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
    {
        Newtonsoft.Json.Serialization.JsonProperty property = base.CreateProperty(member, memberSerialization);
        
        if (property.PropertyName == "attackWeights")
        {
            property.ShouldSerialize = instance => false;
        }
        
        return property;
    }
}

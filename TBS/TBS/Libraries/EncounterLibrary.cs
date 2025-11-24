using Game.Class;
using System.Net.Http.Headers;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

public static class EncounterLibrary
{
    private static Random rng = new Random();

    #region Treasure Encounters

    public static Encounter FoundCoins = new Encounter(
        "FoundCoins",                                    // Unique identifier for this encounter
        false,                                           // IsEnemyEncounter - false means no combat
        "You found some Rai on the ground!",           // Description shown to player
        null,                                            // Enemies list - null for non-combat encounters
        (player) => {                                    // OnEncounter action - custom logic executed when encounter triggers
            int coins = rng.Next(1, 4);                 // Generate random gold amount between 1-2
            player.money += coins;                       // Add gold to player's money
            MainUI.WriteInMainArea($"You gained {coins} Rai!");  // Display result to player
            Program.SavePlayer();                        // Save player state to persist changes
        },
        EncounterType.Treasure                           // Type categorization for organization
    );

    public static Encounter FoundTreasure = new Encounter(
        "FoundTreasure",
        false,
        "You discover a hidden treasure chest!",
        null,
        (player) => {
            int coins = rng.Next(3, 8);
            int exp = rng.Next(5, 15);
            player.money += coins;
            player.exp += exp;
            MainUI.WriteInMainArea($"You gained {coins} Rai and {exp} experience!");
            Program.SavePlayer();
        },
        EncounterType.Treasure
    );

    #endregion

    #region Trap/Hazard Encounters
    public static Encounter LostCoins = new Encounter(
        "LostCoins",
        false,
        "You realize your Rai are missing! A pickpocket must have struck!",
        null,
        (player) => {
            int lostCoins = Math.Min(rng.Next(2, 6), player.money);
            player.money -= lostCoins;
            MainUI.WriteInMainArea($"You lost {lostCoins} Rai!");
            Program.SavePlayer();
        },
        EncounterType.Trap
    );

    public static Encounter FallenIntoTrap = new Encounter(
        "FallenIntoTrap",
        false,
        "You step on a hidden trap! Spikes shoot up from the ground!",
        null,
        (player) => {
            int damage = rng.Next(5, 15);
            player.HP -= damage;
            MainUI.WriteInMainArea($"You took {damage} damage! HP: {Math.Max(0, player.HP)}/{player.maxHP}");
            Program.SavePlayer();
            Program.CheckPlayerDeath();
        },
        EncounterType.Trap
    );

    #endregion

    #region Combat Encounters - Basic/Starter Areas
    public static Encounter BanditFight = new Encounter(
        "BanditFight",
        true,
        "A solo bandit jumps out from behind the trees!",
        new List<Enemy> { EnemyLibrary.Thug},
        null,
        EncounterType.Combat
    );

    public static Encounter BanditAmbush = new Encounter(
        "BanditAmbush",
        true,
        "Bandits jump out from behind the trees!",
        new List<Enemy> { EnemyLibrary.Thug, EnemyLibrary.Thug, EnemyLibrary.Thug, EnemyLibrary.Thug},
        null,
        EncounterType.Combat
    );

    public static Encounter WildGoblin = new Encounter(
        "WildGoblin",
        true,
        "A wild Goblin appears from the bushes!",
        new List<Enemy> { EnemyLibrary.Goblin },
        null,
        EncounterType.Combat
    );

    public static Encounter GoblinPack = new Encounter(
        "GoblinPack",
        true,
        "A pack of Goblins surrounds you!",
        new List<Enemy> { EnemyLibrary.Goblin, EnemyLibrary.Goblin, EnemyLibrary.Goblin },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Forest Regions
    public static Encounter ForestSpiderNest = new Encounter(
        "ForestSpiderNest",
        true,
        "You stumble into a Forest Spider nest!",
        new List<Enemy> { EnemyLibrary.ForestSpider, EnemyLibrary.ForestSpider, EnemyLibrary.ForestSpider },
        null,
        EncounterType.Combat
    );

    public static Encounter DireWolfHunt = new Encounter(
        "DireWolfHunt",
        true,
        "A Dire Wolf stalks you through the trees!",
        new List<Enemy> { EnemyLibrary.DireWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter DireWolfPack = new Encounter(
        "DireWolfPack",
        true,
        "A pack of Dire Wolves surrounds you!",
        new List<Enemy> { EnemyLibrary.DireWolf, EnemyLibrary.DireWolf },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Coastal Regions
    public static Encounter SeaBanditRaid = new Encounter(
        "SeaBanditRaid",
        true,
        "Sea Bandits attack from the shore!",
        new List<Enemy> { EnemyLibrary.SeaBandit, EnemyLibrary.SeaBandit },
        null,
        EncounterType.Combat
    );

    public static Encounter SmugglerAmbush = new Encounter(
        "SmugglerAmbush",
        true,
        "Smugglers emerge from the shadows!",
        new List<Enemy> { EnemyLibrary.Smuggler, EnemyLibrary.Smuggler },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Fallen Kingdom
    public static Encounter VampireAttack = new Encounter(
        "VampireAttack",
        true,
        "A Vampire Spawn emerges from the shadows!",
        new List<Enemy> { EnemyLibrary.VampireSpawn },
        null,
        EncounterType.Combat
    );

    public static Encounter GhostlyApparition = new Encounter(
        "GhostlyApparition",
        true,
        "A Ghostly Apparition appears!",
        new List<Enemy> { EnemyLibrary.GhostlyApparition },
        null,
        EncounterType.Combat
    );

    public static Encounter SkeletonWarriors = new Encounter(
        "SkeletonWarriors",
        true,
        "A couple Skeleton Warrior appears!",
        new List<Enemy> { EnemyLibrary.SkeletonWarrior, EnemyLibrary.SkeletonWarrior, EnemyLibrary.SkeletonWarrior },
        null,
        EncounterType.Combat
    );

    public static Encounter KingdomGuardPatrol = new Encounter(
        "KingdomGuardPatrol",
        true,
        "Kingdom Guards spot you as an intruder!",
        new List<Enemy> { EnemyLibrary.KingdomGuard, EnemyLibrary.KingdomGuard },
        null,
        EncounterType.Combat
    );

    public static Encounter CorruptedKnightBattle = new Encounter(
        "CorruptedKnightBattle",
        true,
        "A Corrupted Knight challenges you!",
        new List<Enemy> { EnemyLibrary.CorruptedKnight },
        null,
        EncounterType.Combat
    );

    public static Encounter RuinedGolemAwakens = new Encounter(
        "RuinedGolemAwakens",
        true,
        "An ancient Ruined Golem awakens!",
        new List<Enemy> { EnemyLibrary.RuinedGolem },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Frozen Regions
    public static Encounter IceWolfPack = new Encounter(
        "IceWolfPack",
        true,
        "A pack of Ice Wolves emerges from the blizzard!",
        new List<Enemy> { EnemyLibrary.IceWolf, EnemyLibrary.IceWolf, EnemyLibrary.IceWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter FrostTrollAmbush = new Encounter(
        "FrostTrollAmbush",
        true,
        "A massive Frost Troll blocks your path!",
        new List<Enemy> { EnemyLibrary.FrostTroll },
        null,
        EncounterType.Combat
    );

    public static Encounter IceMageEncounter = new Encounter(
        "IceMageEncounter",
        true,
        "An Ice Mage materializes from the frozen mist!",
        new List<Enemy> { EnemyLibrary.IceMage, EnemyLibrary.IceWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter SnowWraithAttack = new Encounter(
        "SnowWraithAttack",
        true,
        "A Snow Wraith descends upon you!",
        new List<Enemy> { EnemyLibrary.SnowWraith },
        null,
        EncounterType.Combat
    );

    public static Encounter FrozenHorde = new Encounter(
        "FrozenHorde",
        true,
        "You're surrounded by frozen creatures!",
        new List<Enemy> { EnemyLibrary.IceWolf, EnemyLibrary.IceWolf, EnemyLibrary.IceMage },
        null,
        EncounterType.Combat
    );
    public static Encounter FrozenGolemHealer = new Encounter(
    "FrozenGolem Healer",
    true,
    "You're surrounded by frozen creatures!",
    new List<Enemy> { EnemyLibrary.Healer, EnemyLibrary.Healer, EnemyLibrary.GlacierGolem },
    null,
    EncounterType.Combat
);
    #endregion

    #region Event Encounters
    public static Encounter StrangeMushrooms = new Encounter(
        "StrangeMushrooms",
        false,
        "You stumble upon some strange mushrooms.",
        null,
        (player) => {
            MainUI.WriteInMainArea("Eat the mushrooms? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int roll = rng.Next(1, 101);
                if (roll <= 50)
                {
                    int heal = rng.Next(10, 25);
                    player.HP = Math.Min(player.maxHP, player.HP + heal);
                    MainUI.WriteInMainArea($"The mushrooms were healing! You recovered {heal} HP!");
                }
                else
                {
                    int damage = rng.Next(5, 15);
                    player.HP -= damage;
                    MainUI.WriteInMainArea($"The mushrooms were poisonous! You took {damage} damage!");
                }
                Program.SavePlayer();
                Program.CheckPlayerDeath();
            }
            else
            {
                MainUI.WriteInMainArea("You decide not to risk it and move on.");
            }
        },
        EncounterType.Event
    );

    public static Encounter MysteriousShrine = new Encounter(
        "MysteriousShrine",
        false,
        "You find a mysterious shrine",
        null,
        (player) => {
            MainUI.WriteInMainArea("Pray at the shrine? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int roll = rng.Next(1, 101);
                if (roll <= 70)
                {
                    int exp = rng.Next(1, 50);
                    player.exp += exp;
                    MainUI.WriteInMainArea($"The shrine blesses you! You gained {exp} experience!");
                }
                if (roll == 89)
                {
                    player.HP -= player.HP-1;
                }
                else
                {
                    MainUI.WriteInMainArea("Nothing happens...");
                }
                Program.SavePlayer();
            }
            else
            {
                MainUI.WriteInMainArea("You respectfully pass by the shrine.");
            }
        },
        EncounterType.Mystery
    );

    public static Encounter WanderingMerchant = new Encounter(
        "WanderingMerchant",
        false,
        "A wandering merchant offers you a mysterious potion",
        null,
        (player) => {
            MainUI.WriteInMainArea("Buy the potion for 15 Rai? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                if (player.money >= 15)
                {
                    player.money -= 15;
                    int heal = rng.Next(20, 40);
                    player.HP = Math.Min(player.maxHP, player.HP + heal);
                    MainUI.WriteInMainArea($"You bought the potion and recovered {heal} HP!");
                    Program.SavePlayer();
                }
                else
                {
                    MainUI.WriteInMainArea("You don't have enough Rai!");
                }
            }
            else
            {
                MainUI.WriteInMainArea("You decline the merchant's offer.");
            }
        },
        EncounterType.Merchant
    );

    public static Encounter RoadGambling = new Encounter(
        "RoadGambling",
        false,
        "An old man offers to gamble with you on a coin flip",
        null,
        (player) => {
            MainUI.WriteInMainArea("Accept his offer? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                MainUI.WriteInMainArea($"how much do you want to bet? current Rai: {Program.player.money} \nYou're betting on heads");
                int.TryParse(Console.ReadLine(), out int bet);
                if (bet == null || bet < 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                    return;
                }
                else if (bet > Program.player.money)
                {
                    MainUI.WriteInMainArea("\nyou dont have that much Rai\n ");
                    return;
                }
                MainUI.WriteInMainArea($"you bet: {bet}");
                Program.player.money -= bet;
                Program.SavePlayer();

                Random rand = new Random();
                int coin = rand.Next(1, 3);
                if (coin == 1)
                {
                    MainUI.WriteInMainArea($"The coin landed on heads, you win {bet} Rai!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("The coin landed on tails, you lost...");
                }


            }
            else
            {
                MainUI.WriteInMainArea("You respectfully decline his offer.");
            }
        },
        EncounterType.Event
    );

    public static Encounter FallingFish = new Encounter(
        "FallingFish",
        false,
        "A fish slings out of the pond and lands in front of you",
        null,
        (player) => {
            MainUI.WriteInMainArea("Would you like to pick it up? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                
                Random rand = new Random();
                int slap = rand.Next(1, 6);
                if (slap == 5)
                {
                    player.HP -= 1;
                    MainUI.WriteInMainArea($"The fish slaps your hand away with its fins! You took 1 damage!");
                }
                else
                {
                    MainUI.WriteInMainArea("You successfully pick up the fish without issues!");
                    Inventory.AddItem(ItemLibrary.fish,1);
                }


            }
            else
            {
                MainUI.WriteInMainArea("You don't pick the fish up out of respect");
            }
        },
        EncounterType.Mystery
    );

    public static Encounter LearnFirstAid = new Encounter(
        "LearnFirstAid",
        false,
        "You see a doctor on the side of the road",
        null,
        (player) => {
            if (player.HP < player.maxHP * 0.7 && !player.ownedAttacks.Contains(AttackLibrary.FirstAid))
            {
                {
                    MainUI.WriteInMainArea("The doctor sees you're wounded and offers to teach you first aid\n");
                    MainUI.WriteInMainArea("Learn the move first aid? (y/n): ");
                    string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
                    if (choice == "y" || choice == "yes")
                    {
                        MainUI.WriteInMainArea("The doctor teaches you first aid!");
                        AttackManager atkmanager = new AttackManager(Program.player);
                        atkmanager.LearnAttack(AttackLibrary.FirstAid);
                    }
                    else
                    {
                        MainUI.WriteInMainArea("You respectfully decline the doctors offer.");
                    }
                }
            }
        },
        EncounterType.Mystery
    );



    public static Encounter Wolfsensei = new Encounter(
        "Wolfsensei",
        false,
        "You stumble into a.. Friendly wolf perhaps..",
        null,
        (player) => {
            MainUI.WriteInMainArea("Do you talk to him? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int damage = rng.Next(5, 15);
                player.HP -= damage;
                MainUI.WriteInMainArea($"The wolf bites you and deals {damage} damage!");
                MainUI.WriteInMainArea($"The wolf says you could learn their form of combat!");
                Program.SavePlayer();
                Program.CheckPlayerDeath();
                MainUI.WriteInMainArea("Do you accept his offer? (y/n): ");
                string choice2 = Console.ReadKey(true).KeyChar.ToString().ToLower();
                if (choice2 == "y" || choice2 == "yes")
                {
                    AttackManager atkmanager = new AttackManager(Program.player);
                    atkmanager.LearnAttack(AttackLibrary.Bite);
                }
                else
                {
                    MainUI.WriteInMainArea("You respectfully decline the wolf's offer.");
                }
            }
            else
            {
                MainUI.WriteInMainArea("You decide not to risk it and move on.");
            }
        },
        EncounterType.Event
    );
    public static Encounter Snowman = new Encounter(
     "Snowman",
     false,
     "You see a snowman in the distance..",
     null,
     (player) => {
         MainUI.WriteInMainArea("Do you approach it? (y/n): ");
         string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
         if (choice == "y" || choice == "yes")
         {
             int number = rng.Next(1, 3);
             if (number == 1)
             {
                 MainUI.WriteInMainArea($"The snowman begins moving, and asks if u wanna see something funny.");
                 MainUI.WriteInMainArea("Do you accept his offer? (y/n): ");
                 string choice2 = Console.ReadKey(true).KeyChar.ToString().ToLower();
                 if (choice2 == "y" || choice2 == "yes")
                 {
                     int number2 = rng.Next(1, 3);
                     if (number2 == 1)
                     {
                         int damage = rng.Next(15, 50);
                         player.HP -= damage;
                         MainUI.WriteInMainArea($"The snowman throws a snowball at you and deals {damage} damage!");
                         Program.SavePlayer();
                         Program.CheckPlayerDeath();
                     }
                     else
                     {
                         AttackManager atkmanager = new AttackManager(Program.player);
                         atkmanager.LearnAttack(AttackLibrary.Snowball);
                     }
                 }
                 else
                 {
                     MainUI.WriteInMainArea("You respectfully decline the snowman's offer.");
                 }
             }
             else
             {
                 MainUI.WriteInMainArea($"You walk up to the snowman, just to see its a normal snowman, like any other.");
             }
         }
         else
         {
             MainUI.WriteInMainArea("You decide not to risk it and move on.");
         }
     },
     EncounterType.Event
 );
    #endregion
}

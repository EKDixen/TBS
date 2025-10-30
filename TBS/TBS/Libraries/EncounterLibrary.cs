using Game.Class;

public static class EncounterLibrary
{
    private static Random rng = new Random();

    #region Treasure Encounters

    public static Encounter FoundCoins = new Encounter(
        "FoundCoins",                                    // Unique identifier for this encounter
        false,                                           // IsEnemyEncounter - false means no combat
        "You found some coins on the ground!",           // Description shown to player
        null,                                            // Enemies list - null for non-combat encounters
        (player) => {                                    // OnEncounter action - custom logic executed when encounter triggers
            int coins = rng.Next(5, 20);                 // Generate random gold amount between 5-20
            player.money += coins;                       // Add gold to player's money
            MainUI.WriteInMainArea($"You gained {coins} gold!");  // Display result to player
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
            int coins = rng.Next(20, 50);
            int exp = rng.Next(5, 15);
            player.money += coins;
            player.exp += exp;
            MainUI.WriteInMainArea($"You gained {coins} gold and {exp} experience!");
            Program.SavePlayer();
        },
        EncounterType.Treasure
    );

    #endregion

    #region Trap/Hazard Encounters
    public static Encounter LostCoins = new Encounter(
        "LostCoins",
        false,
        "You realize your coins are missing! A pickpocket must have struck!",
        null,
        (player) => {
            int lostCoins = Math.Min(rng.Next(5, 15), player.money);
            player.money -= lostCoins;
            MainUI.WriteInMainArea($"You lost {lostCoins} gold!");
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

    #region Combat Encounters
    public static Encounter WildGoblin = new Encounter(
        "WildGoblin",
        true,
        "A wild Goblin appears from the bushes!",
        new List<Enemy> { EnemyLibrary.Goblin },
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

    public static Encounter BanditFight = new Encounter(
        "BanditFight",
        true,
        "A solo bandit jumps out from behind the trees!",
        new List<Enemy> { EnemyLibrary.Thug},
        null,
        EncounterType.Combat
    );

    public static Encounter VampireAttack = new Encounter(
        "VampireAttack",
        true,
        "A Vampire Spawn emerges from the shadows!",
        new List<Enemy> { EnemyLibrary.VampireSpawn },
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

    #region Event Encounters
    public static Encounter StrangeMushrooms = new Encounter(
        "StrangeMushrooms",
        false,
        "You stumble upon some strange mushrooms.",
        null,
        (player) => {
            MainUI.WriteInMainArea("Eat the mushrooms? (y/n): ");
            string choice = Console.ReadKey().KeyChar.ToString().ToLower();
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
            string choice = Console.ReadKey().KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int roll = rng.Next(1, 101);
                if (roll <= 70)
                {
                    int exp = rng.Next(10, 30);
                    player.exp += exp;
                    MainUI.WriteInMainArea($"The shrine blesses you! You gained {exp} experience!");
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
            MainUI.WriteInMainArea("Buy the potion for 15 gold? (y/n): ");
            string choice = Console.ReadKey().KeyChar.ToString().ToLower();
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
                    MainUI.WriteInMainArea("You don't have enough gold!");
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
        "An old man offers gamble on a coin flip",
        null,
        (player) => {
            MainUI.WriteInMainArea("Accept his offer? (y/n): ");
            string choice = Console.ReadKey().KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                MainUI.WriteInMainArea($"how much do you want to bet? current cash: {Program.player.money} \nYou're betting on heads");
                int.TryParse(Console.ReadLine(), out int bet);
                if (bet == null || bet < 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                    return;
                }
                else if (bet > Program.player.money)
                {
                    MainUI.WriteInMainArea("\nyou dont have that much money\n ");
                    return;
                }
                MainUI.WriteInMainArea($"you bet: {bet}");
                Program.player.money -= bet;
                Program.SavePlayer();

                Random rand = new Random();
                int coin = rand.Next(1, 3);
                if (coin == 1)
                {
                    MainUI.WriteInMainArea($"The coin landed on heads, you win {bet} cash!");
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



    #endregion
}

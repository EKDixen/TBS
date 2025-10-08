using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class CombatManager
{
    private Player player;
    private List<Enemy> enemies;
    private List<Combatant> combatants;
    private const double ActionThreshold = 100.0;
    private readonly Random rng = new Random();
    private readonly Dictionary<Combatant, int> stunnedTurns = new();

    public CombatManager(Player p, List<Enemy> initialEnemies)
    {
        player = p;
        enemies = initialEnemies;

        player.IsPlayer = true;
        foreach (var e in enemies) e.IsPlayer = false;

        combatants = new List<Combatant>();
        combatants.Add(player);
        combatants.AddRange(enemies);

        foreach (var combatant in combatants)
        {
            combatant.ActionGauge = 0;
            stunnedTurns[combatant] = 0;
            if (combatant.maxHP <= 0) combatant.maxHP = combatant.HP;
            if (combatant.maxHP > 0 && combatant.HP > combatant.maxHP) combatant.HP = combatant.maxHP;
        }
    }

    public void StartCombat()
    {
        Console.WriteLine("--- Combat Started! ---");

        while (player.IsAlive() && enemies.Any(e => e.IsAlive()))
        {
            var readyCombatants = GetReadyCombatants();

            if (readyCombatants.Count == 0)
            {
                AdvanceActionGauges();
                continue;
            }

            Combatant currentActor;
            var maxGauge = readyCombatants.Max(c => c.ActionGauge);
            var topCombatants = readyCombatants.Where(c => c.ActionGauge == maxGauge).ToList();

            if (topCombatants.Count == 1)
            {
                currentActor = topCombatants[0];
            }
            else
            {
                // WOOP WOOP TIERBREAKER (2 personer med samme score over 100)
                Console.WriteLine("\n--- TIE BREAKER! ---");
                Console.WriteLine("Multiple combatants are ready! Let's settle this with Rock, Paper, Scissors.");
                currentActor = ResolveTieBreaker(topCombatants);
                Console.WriteLine($"{currentActor.name} wins the tie-breaker and acts first!");
            }

            TakeTurn(currentActor);
        }

        if (player.IsAlive())
        {
            int totalMoney = enemies.Sum(e => e.money);
            int totalExp = enemies.Sum(e => e.exp);
            player.money += totalMoney;
            player.exp += totalExp;
            Console.WriteLine("\n--- VICTORY! ---");
            Console.WriteLine($"Rewards: +{totalExp} EXP, +{totalMoney} money");
            Program.SavePlayer();
            Thread.Sleep(800);
        }
        else
        {
            Console.WriteLine("\nYou died.");
            Program.SavePlayer();
            Thread.Sleep(800);
        }
    }

    private void AdvanceActionGauges()
    {
        double timeToNextTurn = double.MaxValue;
        foreach (var combatant in combatants.Where(c => c.IsAlive()))
        {
            if (combatant.speed <= 0) continue; // Avoid division by zero

            double timeNeeded = (ActionThreshold - combatant.ActionGauge) / combatant.speed;
            if (timeNeeded < timeToNextTurn)
            {
                timeToNextTurn = timeNeeded;
            }
        }

        // No inf loop ting ting (Failsafe)
        if (double.IsInfinity(timeToNextTurn) || timeToNextTurn <= 0)
        {
            timeToNextTurn = 1;
        }

        foreach (var combatant in combatants.Where(c => c.IsAlive()))
        {
            combatant.ActionGauge += combatant.speed * timeToNextTurn;
        }
    }

    private List<Combatant> GetReadyCombatants()
    {
        return combatants.Where(c => c.IsAlive() && c.ActionGauge >= ActionThreshold).ToList();
    }

    private Combatant ResolveTieBreaker(List<Combatant> tiedCombatants)
    {
        // Hvis spilleren ikke er en del af tiebreaker vælger den bare random
        if (!tiedCombatants.Contains(player))
        {
            return tiedCombatants[rng.Next(tiedCombatants.Count)];
        }

        string[] choices = { "Rock", "Paper", "Scissors" };
        int playerChoice = -1;

        while (playerChoice == -1)
        {
            Console.WriteLine("Choose your move: Rock, Paper, or Scissors");
            string input = Console.ReadLine().ToLower();

            switch (input)
            {
                case "rock":
                    playerChoice = 0;
                    break;
                case "paper":
                    playerChoice = 1;
                    break;
                case "scissors":
                    playerChoice = 2;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter Rock, Paper, or Scissors.");
                    break;
            }
        }

        Console.WriteLine($"You chose {choices[playerChoice]}.");

        var enemyChoices = new Dictionary<Combatant, int>();
        foreach (var enemy in tiedCombatants.Where(c => !c.IsPlayer))
        {
            int enemyChoice = rng.Next(0, 3);
            enemyChoices.Add(enemy, enemyChoice);
            Console.WriteLine($"{enemy.name} chose {choices[enemyChoice]}.");
        }

        bool playerWinsOverall = true;
        foreach (var enemyChoice in enemyChoices.Values)
        {
            if (playerChoice == enemyChoice) // Draw
            {
                continue;
            }
            if ((playerChoice == 0 && enemyChoice == 1) ||
                (playerChoice == 1 && enemyChoice == 2) ||
                (playerChoice == 2 && enemyChoice == 0))
            {
                playerWinsOverall = false;
                break;
            }
        }

        if (playerWinsOverall)
        {
            bool onlyDraws = enemyChoices.Values.All(ec => ec == playerChoice);
            if (onlyDraws)
            {
                Console.WriteLine("It's a complete draw! Let's go again.");
                return ResolveTieBreaker(tiedCombatants);
            }
            else
            {
                // Player vinder mindst mod 1 og taber ingen
                Console.WriteLine("You win the tie-breaker!");
                return player;
            }
        }
        else
        {
            // Player taber til mindst en fjende
            var winningEnemies = new List<Combatant>();
            foreach (var entry in enemyChoices)
            {
                if ((playerChoice == 0 && entry.Value == 1) ||
                    (playerChoice == 1 && entry.Value == 2) ||
                    (playerChoice == 2 && entry.Value == 0))
                {
                    winningEnemies.Add(entry.Key);
                }
            }

            // Vælg en fjende til at andgribe
            var winner = winningEnemies[rng.Next(winningEnemies.Count)];
            Console.WriteLine($"{winner.name} wins the tie-breaker!");
            return winner;
            }
        }
            
            private bool RollChance(int chance)
            {
            chance = Math.Clamp(chance, 0, 100);
            return rng.Next(0, 100) < chance;
            }
            
            private int ComputeDamage(Combatant attacker, Combatant defender, int baseValue, out bool dodged, out bool crit, out bool stunnedApplied, out int rawBeforeArmor, out int armorApplied, out double mult)
            {
            dodged = false; crit = false; stunnedApplied = false;
            rawBeforeArmor = 0; armorApplied = 0; mult = 1.0;
            
            int dodgeChance = Math.Clamp(defender.dodge - attacker.dodgeNegation, 0, 100);
            if (RollChance(dodgeChance))
            {
            dodged = true;
            return 0;
            }
            
            int critChance = Math.Clamp(attacker.critChance, 0, 100);
            if (RollChance(critChance))
            {
            crit = true;
            mult += attacker.critDamage / 100.0;
            }
            
            rawBeforeArmor = (int)Math.Round(baseValue * mult);
            armorApplied = Math.Max(0, defender.armor);
            int dmg = Math.Max(0, rawBeforeArmor - armorApplied);
            
            int stunChance = Math.Clamp(attacker.stun - defender.stunNegation, 0, 100);
            if (stunChance > 0 && RollChance(stunChance))
            {
            stunnedApplied = true;
            }
            
            return dmg;
            }
            
            private void ExecuteAttackSingle(Combatant attacker, Attack attack, Combatant defender)
            {
            Console.WriteLine($"{attacker.name} uses {attack.name} on {defender.name}!");
            Thread.Sleep(400);
            
            foreach (var effect in attack.effects)
            {
            if (effect.targetType == "allEnemies") continue; // skip AoE-flagged effects in single-target
            
            if (effect.type == "damage")
            {
            int before = defender.HP;
            bool dodged, crit, stunInflicted;
            int dmg = ComputeDamage(attacker, defender, effect.value, out dodged, out crit, out stunInflicted, out int rawBeforeArmor, out int armorApplied, out double mult);
            if (dodged)
            {
            Console.WriteLine($"{defender.name} dodged the attack!");
            Thread.Sleep(350);
            continue;
            }
            defender.HP -= dmg;
            int after = Math.Max(defender.HP, 0);
            Console.WriteLine($"{defender.name} takes {dmg} damage{(crit ? " (CRITICAL!)" : "")} ({before} -> {after})");
            Console.WriteLine($"  Armor applied: base {effect.value} x {mult:0.##} = {rawBeforeArmor}, armor {armorApplied} -> {dmg}");
            Thread.Sleep(350);
            if (stunInflicted)
            {
            stunnedTurns[defender] = Math.Max(stunnedTurns.ContainsKey(defender) ? stunnedTurns[defender] : 0, 1);
            Console.WriteLine($"{defender.name} is stunned!");
            Thread.Sleep(350);
            }
            }
            else if (effect.type == "heal")
            {
            var target = (effect.targetType == "self") ? attacker : defender;
            int before = target.HP;
            int max = target.maxHP > 0 ? target.maxHP : int.MaxValue;
            target.HP = Math.Min(before + effect.value, max);
            int healed = target.HP - before;
            Console.WriteLine($"{target.name} heals {healed} ({before} -> {target.HP})");
            Thread.Sleep(350);
            }
            else
            {
            var target = (effect.targetType == "self") ? attacker : defender;
            effect.Apply(attacker, defender);
            string sign = effect.value >= 0 ? "+" : "";
            string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
            Console.WriteLine($"{target.name}: {effect.type} {sign}{effect.value}{dur}");
            Thread.Sleep(300);
            }
            }
            }
            
            private void ExecuteAttackAoE(Combatant attacker, Attack attack, List<Combatant> defenders)
            {
            Console.WriteLine($"{attacker.name} uses {attack.name} on ALL enemies!");
            Thread.Sleep(400);
            
            foreach (var effect in attack.effects)
            {
            if (effect.targetType != "allEnemies") continue;
            
            if (effect.type == "damage")
            {
            foreach (var d in defenders)
            {
            int before = d.HP;
            bool dodged, crit, stunInflicted;
            int dmg = ComputeDamage(attacker, d, effect.value, out dodged, out crit, out stunInflicted, out int rawBeforeArmor, out int armorApplied, out double mult);
            if (dodged)
            {
            Console.WriteLine($"{d.name} dodged the attack!");
            Thread.Sleep(250);
            continue;
            }
            d.HP -= dmg;
            int after = Math.Max(d.HP, 0);
            Console.WriteLine($"{d.name} takes {dmg} damage{(crit ? " (CRITICAL!)" : "")} ({before} -> {after})");
            Console.WriteLine($"  Armor applied: base {effect.value} x {mult:0.##} = {rawBeforeArmor}, armor {armorApplied} -> {dmg}");
            Thread.Sleep(250);
            if (stunInflicted)
            {
            stunnedTurns[d] = Math.Max(stunnedTurns.ContainsKey(d) ? stunnedTurns[d] : 0, 1);
            Console.WriteLine($"{d.name} is stunned!");
            Thread.Sleep(250);
            }
            }
            }
            else
            {
            foreach (var d in defenders)
            {
            if (effect.type == "heal")
            {
                int before = d.HP;
                int max = d.maxHP > 0 ? d.maxHP : int.MaxValue;
                d.HP = Math.Min(before + effect.value, max);
                int healed = d.HP - before;
                Console.WriteLine($"{d.name} heals {healed} ({before} -> {d.HP})");
                Thread.Sleep(250);
            }
            else
            {
                effect.Apply(attacker, d);
                string sign = effect.value >= 0 ? "+" : "";
                string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
                Console.WriteLine($"{d.name}: {effect.type} {sign}{effect.value}{dur}");
                Thread.Sleep(250);
            }
            }
            }
            }
            }
            
            private void PrintStatus()
            {
            int playerAG = (int)Math.Min(100, Math.Round(player.ActionGauge / ActionThreshold * 100));
            int pHP = Math.Max(player.HP, 0);
            string playerStr = $"{player.name} (You) [{pHP}/{player.maxHP} | AG:{playerAG}%]";
            if (enemies.Count > 0)
            {
            // Prefer the first alive enemy on the first line; otherwise show the first entry
            int primaryIndex = enemies.FindIndex(en => en.IsAlive());
            if (primaryIndex < 0) primaryIndex = 0;
            var primary = enemies[primaryIndex];
            var tag0 = primary.IsAlive() ? "" : " (dead)";
            int e0AG = (int)Math.Min(100, Math.Round(primary.ActionGauge / ActionThreshold * 100));
            int e0HP = Math.Max(primary.HP, 0);
            Console.WriteLine($"{playerStr}    {primary.name}{tag0} [{e0HP}/{primary.maxHP} | AG:{e0AG}%]");
            for (int i = 0; i < enemies.Count; i++)
            {
            if (i == primaryIndex) continue;
            var e = enemies[i];
            var tag = e.IsAlive() ? "" : " (dead)";
            int ag = (int)Math.Min(100, Math.Round(e.ActionGauge / ActionThreshold * 100));
            int eHP = Math.Max(e.HP, 0);
            Console.WriteLine($"{e.name}{tag} [{eHP}/{e.maxHP} | AG:{ag}%]");
            }
            }
            else
            {
            Console.WriteLine(playerStr);
            }
            }

        private void TakeTurn(Combatant actor)
        {
            Console.Clear();
            actor.ActionGauge -= ActionThreshold;
            if (actor.ActionGauge < 0) actor.ActionGauge = 0;
            if (!actor.IsAlive()) return;

            // Show current status
            PrintStatus();
            Console.WriteLine();

            // Stun check
            if (stunnedTurns.TryGetValue(actor, out int stunLeft) && stunLeft > 0)
            {
                Console.WriteLine($"\n{actor.name} is stunned and cannot act this turn!");
                stunnedTurns[actor] = stunLeft - 1;
                Thread.Sleep(600);
                return;
            }

            if (actor == player)
            {
                var moves = player.equippedAttacks.Where(a => a != null).ToList();
                if (moves.Count == 0)
                {
                    Console.WriteLine($"{player.name} has no moves equipped and skips the turn.");
                    return;
                }

                Console.WriteLine("\n--- Your Turn ---");
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {moves[i].name} - {moves[i].GetDescription()}");
                }
                int choice = -1;
                while (true)
                {
                    Console.Write("Choose a move: ");
                    if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= moves.Count)
                        break;
                    Console.WriteLine("Invalid choice. Please enter a valid move number.");
                }
                var chosen = moves[choice - 1];

                bool isAoE = chosen.effects.Any(e => e.targetType == "allEnemies");
                if (isAoE)
                {
                    var allEnemies = enemies.Cast<Combatant>().ToList();
                    Console.WriteLine();
                    ExecuteAttackAoE(player, chosen, allEnemies);
                    Console.WriteLine();
                }
                else
                {
                    while (true)
                    {
                        Console.WriteLine("Choose a target:");
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            var tag = enemies[i].IsAlive() ? "" : " (dead)";
                            Console.WriteLine($"{i + 1}. {enemies[i].name}{tag} [{enemies[i].HP}/{enemies[i].maxHP}]");
                        }
                        Console.Write("Target #: ");
                        if (!int.TryParse(Console.ReadLine(), out int t) || t < 1 || t > enemies.Count)
                        {
                            Console.WriteLine("Invalid target. Please enter a valid target number.");
                            continue;
                        }
                        var target = enemies[t - 1];
                        Console.WriteLine();
                        ExecuteAttackSingle(player, chosen, target);
                        Console.WriteLine();
                        break;
                    }
                }
            }
            else
            {
                var enemy = (Enemy)actor;
                Console.WriteLine($"\n--- {enemy.name}'s Turn ---");
                if (enemy.attacks == null || enemy.attacks.Count == 0)
                {
                    Console.WriteLine($"{enemy.name} hesitates and does nothing.");
                    return;
                }
                var chosen = enemy.attacks[rng.Next(enemy.attacks.Count)];
                bool isAoE = chosen.effects.Any(e => e.targetType == "allEnemies");
                if (isAoE)
                {
                    Console.WriteLine();
                    ExecuteAttackSingle(enemy, chosen, player);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                    ExecuteAttackSingle(enemy, chosen, player);
                    Console.WriteLine();
                }
                            }

            PrintStatus();
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }


    //private void TakeTurn(Combatant actor)
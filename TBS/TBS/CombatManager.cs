using Game.Class;
using System;
using System.Collections.Generic;
using System.Linq;

public class CombatManager
{
    private Player player;
    private List<Enemy> enemies;
    private List<Combatant> combatants;
    private const double ActionThreshold = 100.0;

    public CombatManager(Player p, List<Enemy> initialEnemies)
    {
        player = p;
        enemies = initialEnemies;

        combatants = new List<Combatant>();
        combatants.Add(player);
        combatants.AddRange(enemies);

        foreach (var combatant in combatants)
        {
            combatant.ActionGauge = 0;
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

            //TakeTurn(currentActor);

            enemies.RemoveAll(e => !e.IsAlive());
            combatants.RemoveAll(c => !c.IsAlive() && !c.IsPlayer);
        }

        if (player.IsAlive())
        {
            Console.WriteLine("\n--- VICTORY! ---");
        }
        else
        {
            Console.WriteLine("\n--- DEFEAT! ---");
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
            return tiedCombatants[new Random().Next(tiedCombatants.Count)];
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
            int enemyChoice = new Random().Next(0, 3);
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
            var winner = winningEnemies[new Random().Next(winningEnemies.Count)];
            Console.WriteLine($"{winner.name} wins the tie-breaker!");
            return winner;
        }
    }
}


    //private void TakeTurn(Combatant actor)
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
    private CombatUI ui;

    public static bool playerInCombat = false;

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

        ui = new CombatUI();
        ui.InitializeConsole();
    }

    public void StartCombat()
    {
        playerInCombat = true;

        ui.AddToLog("--- Combat Started! ---");
        ui.RenderCombatScreen(player, combatants);
        Thread.Sleep(1000);

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
                currentActor = topCombatants[rng.Next(topCombatants.Count)];
            }

            TakeTurn(currentActor);
        }

        if (player.IsAlive())
        {
            int totalMoney = enemies.Sum(e => e.money);
            int totalExp = enemies.Sum(e => e.exp);
            player.money += totalMoney;
            player.exp += totalExp;
            
            ui.AddToLog("--- VICTORY! ---");
            ui.AddToLog($"Rewards: +{totalExp} EXP, +{totalMoney} money");
            ui.ClearMainArea();
            ui.WriteInMainArea(8, "+----------------------------------------+");
            ui.WriteInMainArea(9, "�          VICTORY!                      �");
            ui.WriteInMainArea(10, "+----------------------------------------+");
            ui.WriteInMainArea(12, $"Rewards:");
            ui.WriteInMainArea(13, $"  +{totalExp} EXP");
            ui.WriteInMainArea(14, $"  +{totalMoney} Gold");
            ui.WriteInMainArea(16, "Press Enter to continue...");
            ui.RenderCombatScreen(player, combatants);
            
            Program.SavePlayer();
            Console.ReadLine();
        }
        else
        {
                        ui.ClearMainArea();
            ui.WriteInMainArea(8, "+----------------------------------------+");
            ui.WriteInMainArea(9, "�          DEFEAT...                     �");
            ui.WriteInMainArea(10, "+----------------------------------------+");
            ui.WriteInMainArea(12, "You have been defeated in battle...");
            ui.WriteInMainArea(14, "Press Enter to continue...");
            ui.RenderCombatScreen(player, combatants);
            
            Console.ReadLine();
        }

        Console.Clear();
        Console.CursorVisible = true;
        
        // Check if player died
        Program.CheckPlayerDeath();
        playerInCombat = false;
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
            string attackMsg = $"{attacker.name} uses {attack.name} on {defender.name}!";
            ui.AddToLog(attackMsg);
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(400);
            
            foreach (var effect in attack.effects)
            {
            if (effect.targetType == "allEnemies") continue;
            
            if (effect.type == "damage")
            {
            int before = defender.HP;
            bool dodged, crit, stunInflicted;
            int dmg = ComputeDamage(attacker, defender, effect.value, out dodged, out crit, out stunInflicted, out int rawBeforeArmor, out int armorApplied, out double mult);
            if (dodged)
            {
            ui.AddToLog($"{defender.name} dodged the attack!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(350);
            continue;
            }
            defender.HP -= dmg;
            int after = Math.Max(defender.HP, 0);
            ui.AddToLog($"{defender.name} takes {dmg} damage{(crit ? " (CRIT!)" : "")} ({before} -> {after})");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(350);
            if (stunInflicted)
            {
            stunnedTurns[defender] = Math.Max(stunnedTurns.ContainsKey(defender) ? stunnedTurns[defender] : 0, 1);
            ui.AddToLog($"{defender.name} is stunned!");
            ui.RenderCombatScreen(player, combatants);
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
            ui.AddToLog($"{target.name} heals {healed} HP ({before} -> {target.HP})");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(350);
            }
            else
            {
            var target = (effect.targetType == "self") ? attacker : defender;
            effect.Apply(attacker, defender);
            string sign = effect.value >= 0 ? "+" : "";
            string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
            ui.AddToLog($"{target.name}: {effect.type} {sign}{effect.value}{dur}");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(300);
            }
            }
            }
            
            private void ExecuteAttackAoE(Combatant attacker, Attack attack, List<Combatant> defenders)
            {
            ui.AddToLog($"{attacker.name} uses {attack.name} on ALL enemies!");
            ui.RenderCombatScreen(player, combatants);
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
            ui.AddToLog($"{d.name} dodged the attack!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(250);
            continue;
            }
            d.HP -= dmg;
            int after = Math.Max(d.HP, 0);
            ui.AddToLog($"{d.name} takes {dmg} damage{(crit ? " (CRIT!)" : "")} ({before} -> {after})");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(250);
            if (stunInflicted)
            {
            stunnedTurns[d] = Math.Max(stunnedTurns.ContainsKey(d) ? stunnedTurns[d] : 0, 1);
            ui.AddToLog($"{d.name} is stunned!");
            ui.RenderCombatScreen(player, combatants);
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
                ui.AddToLog($"{d.name} heals {healed} HP ({before} -> {d.HP})");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(250);
            }
            else
            {
                effect.Apply(attacker, d);
                string sign = effect.value >= 0 ? "+" : "";
                string dur = effect.duration > 0 ? $" for {effect.duration} turns" : "";
                ui.AddToLog($"{d.name}: {effect.type} {sign}{effect.value}{dur}");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(250);
            }
            }
            }
            }
            }
            
            
        private void TakeTurn(Combatant actor)
        {
            if (!actor.IsAlive()) return;

            ui.RenderCombatScreen(player, combatants);
            ui.AddToLog("");
            // Stun check
            if (stunnedTurns.TryGetValue(actor, out int stunLeft) && stunLeft > 0)
            {
                ui.AddToLog($"{actor.name} is stunned and cannot act!");
                ui.RenderCombatScreen(player, combatants);
                stunnedTurns[actor] = stunLeft - 1;
                Thread.Sleep(800);
                return;
            }

            if (actor == player)
            {
                var moves = player.equippedAttacks.Where(a => a != null).ToList();
                var consumables = player.ownedItems.Where(i => i.type == ItemType.consumable && i.amount > 0).ToList();
                
                if (moves.Count == 0 && consumables.Count == 0)
                {
                    ui.AddToLog($"{player.name} has no moves or items and skips turn.");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(800);
                    return;
                }

                // Display move and item selection in main area
                ui.ClearMainArea();
                ui.WriteInMainArea(0, "--- Your Turn ---");
                ui.WriteInMainArea(1, "");
                
                int lineNum = 2;
                
                // Show attacks
                if (moves.Count > 0)
                {
                    ui.WriteInMainArea(lineNum++, "ATTACKS:");
                    for (int i = 0; i < moves.Count; i++)
                    {
                        ui.WriteInMainArea(lineNum++, $"{i + 1}. {moves[i].name} - {moves[i].GetDescription()}");
                    }
                    lineNum++;
                }
                
                // Show consumables
                int itemStartIndex = moves.Count + 1;
                if (consumables.Count > 0)
                {
                    ui.WriteInMainArea(lineNum++, "ITEMS:");
                    for (int i = 0; i < consumables.Count; i++)
                    {
                        ui.WriteInMainArea(lineNum++, $"{itemStartIndex + i}. {consumables[i].name} x{consumables[i].amount} - {consumables[i].description}");
                    }
                    lineNum++;
                }
                
                ui.SetCursorInMainArea(lineNum);
                Console.Write("Choose action: ");

                int choice = -1;
                int totalOptions = moves.Count + consumables.Count;
                while (true)
                {
                    if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= totalOptions)
                        break;
                    ui.SetCursorInMainArea(lineNum + 1);
                    Console.Write("Invalid choice. Try again: ");
                }
                
                // Check if they chose an attack or item
                if (choice <= moves.Count)
                {
                    // They chose an attack
                    var chosen = moves[choice - 1];

                bool isAoE = chosen.effects.Any(e => e.targetType == "allEnemies");
                    if (isAoE)
                    {
                        var allEnemies = enemies.Cast<Combatant>().ToList();
                        ExecuteAttackAoE(player, chosen, allEnemies);
                    }
                    else
                    {
                        // Target selection
                        ui.ClearMainArea();
                        ui.WriteInMainArea(0, "Choose a target:");
                        ui.WriteInMainArea(1, "");
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            var name = enemies[i].name;
                            var sameNameCount = enemies.Count(e => e.name == name);
                            var indexAmongSame = sameNameCount > 1 ? $" #{enemies.Take(i + 1).Count(e => e.name == name)}" : "";
                            var tag = enemies[i].IsAlive() ? "" : " (dead)";
                            ui.WriteInMainArea(2 + i, $"{i + 1}. {name}{indexAmongSame}{tag} [{enemies[i].HP}/{enemies[i].maxHP}]");
                        }
                        ui.WriteInMainArea(2 + enemies.Count + 1, "");
                        ui.SetCursorInMainArea(2 + enemies.Count + 2);
                        Console.Write("Target #: ");

                        while (true)
                        {
                            if (!int.TryParse(Console.ReadLine(), out int t) || t < 1 || t > enemies.Count)
                            {
                                ui.SetCursorInMainArea(2 + enemies.Count + 3);
                                Console.Write("Invalid. Try again: ");
                                continue;
                            }
                            var target = enemies[t - 1];
                            ExecuteAttackSingle(player, chosen, target);
                            break;
                        }
                    }
                }
                else
                {
                    // They chose an item
                    var chosenItem = consumables[choice - moves.Count - 1];
                    UseConsumableInCombat(player, chosenItem);
                }
            }
            else
            {
                var enemy = (Enemy)actor;
                ui.ClearMainArea();
                ui.WriteInMainArea(0, $"--- {enemy.name}'s Turn ---");
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(500);

                if (enemy.attacks == null || enemy.attacks.Count == 0)
                {
                    ui.AddToLog($"{enemy.name} hesitates and does nothing.");
                    ui.RenderCombatScreen(player, combatants);
                    Thread.Sleep(800);
                    return;
                }
                var chosen = enemy.attacks[rng.Next(enemy.attacks.Count)];
                ExecuteAttackSingle(enemy, chosen, player);
            }
            actor.ActionGauge -= ActionThreshold;
            if (actor.ActionGauge < 0) actor.ActionGauge = 0;
            Thread.Sleep(300);
            ui.RenderCombatScreen(player, combatants);
            ui.WriteInMainArea(12, "");
            ui.WriteInMainArea(13, "Press Enter to continue...");
            ui.SetCursorInMainArea(22);
            Console.ReadLine();
        }

        private void UseConsumableInCombat(Player player, Item item)
        {
            ui.AddToLog($"{player.name} uses {item.name}!");
            ui.RenderCombatScreen(player, combatants);
            Thread.Sleep(400);

            // Apply item effects
            foreach (var stat in item.stats)
            {
                switch (stat.Key)
                {
                    case "HP":
                        int before = player.HP;
                        player.HP = Math.Min(player.maxHP, player.HP + stat.Value);
                        int healed = player.HP - before;
                        ui.AddToLog($"{player.name} heals {healed} HP ({before} -> {player.HP})");
                        break;
                    case "DMG":
                        player.DMG += stat.Value;
                        ui.AddToLog($"{player.name} DMG {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                    case "speed":
                        player.speed += stat.Value;
                        ui.AddToLog($"{player.name} speed {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                    case "armor":
                        player.armor += stat.Value;
                        ui.AddToLog($"{player.name} armor {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                    case "dodge":
                        player.dodge += stat.Value;
                        ui.AddToLog($"{player.name} dodge {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                    case "critChance":
                        player.critChance += stat.Value;
                        ui.AddToLog($"{player.name} crit chance {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                    case "critDamage":
                        player.critDamage += stat.Value;
                        ui.AddToLog($"{player.name} crit damage {(stat.Value >= 0 ? "+" : "")}{stat.Value}");
                        break;
                }
                ui.RenderCombatScreen(player, combatants);
                Thread.Sleep(300);
            }

            // Consume the item
            item.amount--;
            if (item.amount <= 0)
            {
                player.ownedItems.Remove(item);
            }
            
            Program.SavePlayer();
        }
    }


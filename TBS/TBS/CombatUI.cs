using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Game.Class
{
    public class CombatUI
    {
        private static readonly object consoleLock = new();
        private List<string> combatLog = new List<string>();
        private const int MaxLogLines = 8;

        // Console dimensions
        private const int ConsoleWidth = 120;
        private const int ConsoleHeight = 35;

        // Panel dimensions
        private const int MainAreaWidth = 70;
        private const int RightPanelWidth = ConsoleWidth - MainAreaWidth - 1;
        private const int PlayerPanelHeight = 5;
        private const int LogPanelHeight = 15;
        private const int TurnOrderHeight = ConsoleHeight - PlayerPanelHeight - LogPanelHeight - 3;

        //Windows API
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                        int X, int Y, int cx, int cy, uint uFlags);

        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x00010000;
        const int WS_SIZEBOX = 0x00040000;

        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_FRAMECHANGED = 0x0020;

        public void InitializeConsole()
        {
            try
            {
                IntPtr handle = GetConsoleWindow();
                if (handle != IntPtr.Zero)
                {
                    int style = GetWindowLong(handle, GWL_STYLE);
                    style &= ~WS_MAXIMIZEBOX;
                    style &= ~WS_SIZEBOX;
                    SetWindowLong(handle, GWL_STYLE, style);
                    SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
                                 SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                }

                Console.SetWindowSize(ConsoleWidth, ConsoleHeight);
                Console.SetBufferSize(ConsoleWidth, ConsoleHeight);
                Console.CursorVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not lock console window: {ex.Message}");
                Thread.Sleep(1000);
            }
        }

        public void AddToLog(string message)
        {
            combatLog.Add(message);
            if (combatLog.Count > MaxLogLines)
            {
                combatLog.RemoveAt(0);
            }
        }

        public void ClearLog()
        {
            combatLog.Clear();
        }

        public void RenderCombatScreen(Player player, List<Combatant> allCombatants, string mainAreaContent = "")
        {
            lock (consoleLock)
            {
                Console.Clear();
                Console.CursorVisible = false;

                DrawPlayerPanel(player);
                DrawTurnOrderPanel(allCombatants);
                DrawCombatLogPanel();
                DrawMainArea(mainAreaContent);

                Console.CursorVisible = true;
            }
        }

        private void DrawPlayerPanel(Player player)
        {
            int x = MainAreaWidth + 1;
            int y = 0;

            DrawBox(x, y, RightPanelWidth, PlayerPanelHeight, "Player");

            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write($"{player.name} - Lvl {player.level}");

            Console.SetCursorPosition(x + 2, y + 3);
            DrawHealthBar(player.HP, player.maxHP, RightPanelWidth - 4);
        }

        private void DrawTurnOrderPanel(List<Combatant> allCombatants)
        {
            int x = MainAreaWidth + 1;
            int y = PlayerPanelHeight + 1;

            DrawBox(x, y, RightPanelWidth, TurnOrderHeight, "Turn Order");

            var turnOrder = SimulateNext10Turns(allCombatants);

            // Add numbering for duplicates of enemies/allies
            var nameCount = new Dictionary<string, int>();
            var displayNames = new List<string>();
            
            var aliveCombatants = allCombatants.Where(c => c.IsAlive()).ToList();
            var nameTotals = new Dictionary<string, int>();
            foreach (var c in aliveCombatants)
            {
                if (!nameTotals.ContainsKey(c.name))
                    nameTotals[c.name] = 0;
                nameTotals[c.name]++;
            }

            foreach (var combatant in turnOrder)
            {
                string baseName = combatant.name;
                if (!nameCount.ContainsKey(baseName))
                    nameCount[baseName] = 0;
                
                nameCount[baseName]++;
                int count = nameCount[baseName];
                
                // Only add number if there are multiple with this name
                if (nameTotals.ContainsKey(baseName) && nameTotals[baseName] > 1)
                {
                    displayNames.Add($"{baseName} #{count}");
                }
                else
                {
                    displayNames.Add(baseName);
                }
            }

            int maxLines = Math.Min(10, TurnOrderHeight - 3);
            for (int i = 0; i < Math.Min(turnOrder.Count, maxLines); i++)
            {
                var combatant = turnOrder[i];
                string displayName = displayNames[i];
                string type = combatant.IsPlayer ? "(You)" : 
                             (combatant is Player) ? "(Ally)" : "(Enemy)";

                string arrow = i == 0 ? "->" : "  ";

                Console.SetCursorPosition(x + 2, y + 2 + i);
                string line = $"{arrow}{displayName} {type}";
                
                int hpStart = RightPanelWidth - 15;
                if (line.Length < hpStart)
                {
                    line = line.PadRight(hpStart);
                }
                else
                {
                    line = line.Substring(0, hpStart);
                }

                int currentHP = Math.Max(0, combatant.HP);
                line += $"{currentHP}/{combatant.maxHP}";

                if (line.Length > RightPanelWidth - 4)
                {
                    line = line.Substring(0, RightPanelWidth - 4);
                }

                Console.Write(line);
            }
        }

        private List<Combatant> SimulateNext10Turns(List<Combatant> allCombatants)
        {
            const double ActionThreshold = 100.0;
            var result = new List<Combatant>();
            
            var simulated = allCombatants
                .Where(c => c.IsAlive())
                .Select(c => new { Combatant = c, AG = c.ActionGauge })
                .ToList();

            if (simulated.Count == 0) return result;

            // Simulate next 10 turns
            for (int turn = 0; turn < 10; turn++)
            {
                var maxAG = simulated.Max(s => s.AG);
                
                if (maxAG < ActionThreshold)
                {
                    double timeToNext = double.MaxValue;
                    foreach (var s in simulated)
                    {
                        if (s.Combatant.speed <= 0) continue;
                        double timeNeeded = (ActionThreshold - s.AG) / s.Combatant.speed;
                        if (timeNeeded < timeToNext)
                            timeToNext = timeNeeded;
                    }

                    if (double.IsInfinity(timeToNext) || timeToNext <= 0)
                        timeToNext = 1;

                    simulated = simulated.Select(s => new { 
                        s.Combatant, 
                        AG = s.AG + (s.Combatant.speed * timeToNext) 
                    }).ToList();
                    
                    maxAG = simulated.Max(s => s.AG);
                }

                var ready = simulated.Where(s => s.AG >= ActionThreshold).ToList();
                if (ready.Count == 0) break;

                var actor = ready.OrderByDescending(s => s.AG).First();
                result.Add(actor.Combatant);

                simulated = simulated.Select(s => new {
                    s.Combatant,
                    AG = s.Combatant == actor.Combatant ? s.AG - ActionThreshold : s.AG
                }).ToList();
            }

            return result;
        }

        private void DrawCombatLogPanel()
        {
            int x = 0;
            int y = ConsoleHeight - LogPanelHeight - 1;

            DrawBox(x, y, ConsoleWidth, LogPanelHeight, "Combat Log");

            for (int i = 0; i < combatLog.Count; i++)
            {
                Console.SetCursorPosition(x + 2, y + 2 + i);
                string line = combatLog[i];
                if (line.Length > ConsoleWidth - 4)
                {
                    line = line.Substring(0, ConsoleWidth - 4);
                }
                Console.Write(line);
            }
        }

        private void DrawMainArea(string content)
        {
            DrawBox(0, 0, MainAreaWidth, ConsoleHeight - LogPanelHeight - 2, "Combat");

            if (!string.IsNullOrEmpty(content))
            {
                var lines = content.Split('\n');
                for (int i = 0; i < Math.Min(lines.Length, ConsoleHeight - LogPanelHeight - 4); i++)
                {
                    Console.SetCursorPosition(2, 2 + i);
                    string line = lines[i];
                    if (line.Length > MainAreaWidth - 4)
                    {
                        line = line.Substring(0, MainAreaWidth - 4);
                    }
                    Console.Write(line);
                }
            }
        }

        private void DrawHealthBar(int current, int max, int width)
        {
            int barWidth = width - 15;
            int filled = max > 0 ? (int)((double)current / max * barWidth) : 0;
            filled = Math.Max(0, Math.Min(filled, barWidth));

            Console.Write("HP: ");
            Console.Write(new string('█', filled));
            Console.Write(new string('░', barWidth - filled));
            Console.Write($" {Math.Max(0, current)}/{max}");
        }

        private void DrawBox(int x, int y, int w, int h, string? title = null)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("┌" + new string('─', w - 2) + "┐");

            if (!string.IsNullOrEmpty(title) && title.Length < w - 4)
            {
                Console.SetCursorPosition(x + 2, y);
                Console.Write($" {title} ");
            }

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│" + new string(' ', w - 2) + "│");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("└" + new string('─', w - 2) + "┘");
        }

        public void SetCursorInMainArea(int row, int col = 2)
        {
            lock (consoleLock)
            {
                Console.SetCursorPosition(col, row + 2);
            }
        }

        public void WriteInMainArea(int row, string text, int col = 2)
        {
            lock (consoleLock)
            {
                Console.SetCursorPosition(col, row + 2);
                if (text.Length > MainAreaWidth - 4)
                {
                    text = text.Substring(0, MainAreaWidth - 4);
                }
                Console.Write(text.PadRight(MainAreaWidth - 4));
            }
        }

        public void ClearMainArea()
        {
            lock (consoleLock)
            {
                int maxLines = ConsoleHeight - LogPanelHeight - 4;
                for (int i = 0; i < maxLines; i++)
                {
                    Console.SetCursorPosition(2, 2 + i);
                    Console.Write(new string(' ', MainAreaWidth - 4));
                }
            }
        }
    }
}

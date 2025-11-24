using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Game.Class
{
    public static class MainUI
    {
        private static readonly object consoleLock = new();
        private static int mainAreaCurrentLine = 0;

        public static bool ShowMovesInPlayerPanel = false;

        //main area lines type shit
        private static int maxMainAreaLine = 26;
        private const int mainAreaTopLine = 1;
        private static int mainAreaBottomLine = maxMainAreaLine + mainAreaTopLine;

        // Console dimensions
        private const int ConsoleWidth = 120;
        private const int ConsoleHeight = 35;

        // Panel dimensions
        private const int MainAreaWidth = 80;
        private const int RightPanelWidth = ConsoleWidth - MainAreaWidth - 1;
        private const int PlayerPanelHeight = 20;
        private const int MiniMapHeight = ConsoleHeight - PlayerPanelHeight - 3;

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

        public static void InitializeConsole()
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



        public static void RenderMainMenuScreen(Player player, string mainAreaContent = "")
        {
            lock (consoleLock)
            {
                Console.Clear();
                Console.CursorVisible = false;

                DrawPlayerPanel(player);
                DrawMiniMapPanel();
                DrawMainArea(mainAreaContent);

                Console.CursorVisible = true;
            }
        }

        public static async Task LoopRenderMain()
        {
            while (!CombatManager.playerInCombat)
            {

                lock (consoleLock)
                {
                    //Console.Clear();
                    Console.CursorVisible = false;

                    DrawPlayerPanel(Program.player);
                    DrawMiniMapPanel();

                    Console.CursorVisible = true;
                    SetCursorInMainArea(mainAreaCurrentLine);
                }
                //RenderMainMenuScreen(Program.player);


                await Task.Delay(2000);
            }
        }


        private static void DrawPlayerPanel(Player player)
        {
            int x = MainAreaWidth + 1;
            int y = 0;

            DrawBox(x, y, RightPanelWidth, PlayerPanelHeight, "Player");

            Console.SetCursorPosition(x + 14 - (player.name.Length / 2), y + 2);
            Console.Write($"{player.name} - Lvl {player.level}");

            Console.SetCursorPosition(x + 2, y + 4);
            DrawHealthBar(player.exp, player.level * 100, RightPanelWidth - 4);

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write($"HP       - {player.HP}/{player.maxHP}");
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write($"Rai      - {player.money}");
            Console.SetCursorPosition(x + 2, y + 8);

            Console.Write($"Weight   - {player.inventoryWeight}/{Inventory.freeweight}");
            Console.SetCursorPosition(x + 2, y + 9);
            Console.Write($"Materials- {player.currentMaterialLoad}/{player.baseMaterialCapacity + Inventory.GetBackpackCapacityFromInventory(player)}");
            Console.SetCursorPosition(x + 2, y + 10);
            Console.Write($"Class    - {player.playerClass.name}");
            Console.SetCursorPosition(x + 2, y + 11);
            Console.Write($"Location - {player.currentLocation}");

            // Depending on context, show either equipped items or equipped moves
            if (!ShowMovesInPlayerPanel)
            {
                Console.SetCursorPosition(x + 2, y + 13);
                Console.Write("Equiped items:");
                for (int j = 0; j < player.equippedItems.Capacity; j++)
                {
                    string place = "";
                    switch (j)
                    {
                        case 0: place = "Head"; break;
                        case 1: place = "Body"; break;
                        case 2: place = "Legs"; break;
                        case 3: place = "Feet"; break;
                    }
                    Console.SetCursorPosition(x + 2, y + 14 + j);
                    Console.Write($"{j + 1} ({place}) : {player.equippedItems[j]?.name ?? "Empty"}");
                }
            }
            else
            {
                Console.SetCursorPosition(x + 2, y + 12);
                Console.Write("Equipped moves:");
                for (int j = 0; j < player.equippedAttacks.Count; j++)
                {
                    Console.SetCursorPosition(x + 2, y + 13 + j);
                    string moveName = player.equippedAttacks[j]?.name ?? "Empty";
                    Console.Write($"Slot {j + 1}: {moveName}");
                }
            }
        }

        private static void DrawMiniMapPanel()
        {
            int x = MainAreaWidth + 1;
            int y = PlayerPanelHeight + 1;

            DrawBox(x, y, RightPanelWidth, MiniMapHeight, "MiniMap");


            Minimap.DisplayMinimap(x + 2, y + 2, RightPanelWidth - 6);
        }


        private static void DrawMainArea(string content)
        {
            DrawBox(0, 0, MainAreaWidth, ConsoleHeight - 2, "Main");

            if (!string.IsNullOrEmpty(content))
            {
                var lines = content.Split('\n');

                int limit = Math.Min(lines.Length, maxMainAreaLine + 1);

                for (int i = 0; i < limit; i++)
                {
                    Console.SetCursorPosition(2, mainAreaTopLine + i);
                    string line = lines[i];
                    if (line.Length > MainAreaWidth - 4)
                    {
                        line = line.Substring(0, MainAreaWidth - 4);
                    }
                    Console.Write(line);
                }

                mainAreaCurrentLine = limit;
            }
        }

        private static void DrawHealthBar(int current, int max, int width)
        {
            int barWidth = width - 15;
            int filled = max > 0 ? (int)((double)current / max * barWidth) : 0;
            filled = Math.Max(0, Math.Min(filled, barWidth));

            Console.Write("EXP: ");
            Console.Write(new string('█', filled));
            Console.Write(new string('░', barWidth - filled));
            Console.Write($" {Math.Max(0, current)}/{max}");
        }

        private static void DrawBox(int x, int y, int w, int h, string? title = null)
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

        public static void SetCursorInMainArea(int row, int col = 2)
        {
            lock (consoleLock)
            {
                int absoluteRow = mainAreaTopLine + row;

                // check to prevent cursor from going outside the box
                if (absoluteRow > ConsoleHeight - 2)
                {
                    absoluteRow = ConsoleHeight - 2;
                }

                Console.SetCursorPosition(col, absoluteRow);
            }
        }

        public static void WriteInMainArea(string text, int col = 2)
        {
            lock (consoleLock)
            {
                var lines = text.Split('\n');

                foreach (var line in lines)
                {
                    if (mainAreaCurrentLine > maxMainAreaLine)
                    {
                        // --- THIS IS THE SCROLL LOGIC ---

                        // This copies everything from row 2 up to row 1, 3 to 2, etc.
                        Console.MoveBufferArea(
                            sourceLeft: col,
                            sourceTop: mainAreaTopLine + 1, // The *second* absolute line (y=2)
                            sourceWidth: MainAreaWidth - 4, // The width you're writing
                            sourceHeight: maxMainAreaLine,  // The total height of the area minus one line
                            targetLeft: col,
                            targetTop: mainAreaTopLine      // The *first* absolute line (y=1)
                        );

                        // 2. Clear the last line (which is now a duplicate)
                        string clearLine = "".PadRight(MainAreaWidth - 4);
                        Console.SetCursorPosition(col, mainAreaBottomLine); // Go to the absolute bottom line
                        Console.Write(clearLine);


                        // 3. Reset the current line to be the last line
                        mainAreaCurrentLine = maxMainAreaLine;
                    }

                    // Calculate the absolute row to write on
                    int absoluteRow = mainAreaTopLine + mainAreaCurrentLine;

                    // Write the new line at the correct position
                    Console.SetCursorPosition(col, absoluteRow);
                    string cleanLine = line.Trim();
                    Console.Write(cleanLine.PadRight(MainAreaWidth - 4));

                    // 3. Increment the *relative* "next line" counter
                    mainAreaCurrentLine++;
                }

                // Finally, set the cursor to the *next* available line for user input
                // This is now handled by your LoopRenderMain's call to SetCursorInMainArea
                SetCursorInMainArea(mainAreaCurrentLine);
            }
        }

        public static void ClearMainArea()
        {
            lock (consoleLock)
            {
                // Use maxMainAreaLine + 1 to get the total number of lines (0-28 is 29 lines)
                int maxLines = maxMainAreaLine + 2;
                string clearLine = new string(' ', MainAreaWidth - 4);

                for (int i = 0; i < maxLines; i++)
                {
                    // Use mainAreaTopLine as the starting point
                    Console.SetCursorPosition(2, mainAreaTopLine + i);
                    Console.Write(clearLine);
                }
                mainAreaCurrentLine = 0;
            }
        }
    }
}

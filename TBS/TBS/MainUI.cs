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

            Console.SetCursorPosition(x + 14 - (player.name.Length/2), y + 2);
            Console.Write($"{player.name} - Lvl {player.level}");

            Console.SetCursorPosition(x + 2, y + 4);
            DrawHealthBar(player.exp, player.level * 100, RightPanelWidth - 4);

            Console.SetCursorPosition(x + 2, y + 6);
            Console.Write($"HP       - {player.HP}");
            Console.SetCursorPosition(x + 2, y + 7);
            Console.Write($"Money    - {player.money}");
            Console.SetCursorPosition(x + 2, y + 8);
            Console.Write($"Location - {player.currentLocation.name}");
            Console.SetCursorPosition(x + 2, y + 10);
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
                Console.SetCursorPosition(x + 2, y + 11 + j);
                Console.Write($"{j + 1} ({place}) : {player.equippedItems[j]?.name ?? "Empty"}");
            }
        }

        private static void DrawMiniMapPanel()
        {
            int x = MainAreaWidth + 1;
            int y = PlayerPanelHeight + 1;

            DrawBox(x, y, RightPanelWidth, MiniMapHeight, "MiniMap");


            Minimap.DisplayMinimap(x+2,y+2,RightPanelWidth-6);
        }


        private static void DrawMainArea(string content)
        {
            DrawBox(0, 0, MainAreaWidth, ConsoleHeight - 2, "Main");

            if (!string.IsNullOrEmpty(content))
            {
                var lines = content.Split('\n');
                for (int i = 0; i < Math.Min(lines.Length, ConsoleHeight - 4); i++)
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
                Console.SetCursorPosition(col, row + 2);
            }
        }

        public static void WriteInMainArea(string text, int col = 2)
        {
            lock (consoleLock)
            {
                var lines = text.Split('\n');
                int currentLineOffset = 0;

                foreach (var line in lines)
                {
                    int row = mainAreaCurrentLine + currentLineOffset;

                    Console.SetCursorPosition(2, row + 1);

                    // The .Trim() is important to remove any lingering carriage return
                    string cleanLine = line.Trim();

                    // Write the line and pad it to clear any previous text
                    Console.Write(cleanLine.PadRight(MainAreaWidth - 4));

                    // Move to the next line for the next piece of the string
                    currentLineOffset++;
                    Console.SetCursorPosition(2, row + 2);
                }

                mainAreaCurrentLine += currentLineOffset;
            }
        }

        public static void ClearMainArea()
        {                   
            lock (consoleLock)
            {
                int maxLines = ConsoleHeight - 4;
                for (int i = 0; i < maxLines; i++)
                {
                    Console.SetCursorPosition(2, 1 + i);
                    Console.Write(new string(' ', MainAreaWidth - 4));
                }
                mainAreaCurrentLine = 0;

            }
        }
    }

}

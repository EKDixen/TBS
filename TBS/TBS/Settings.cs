using Game.Class;

public class Settings
{
    public Settings() { }



    public void ChangeTextColor()
    {
        Console.WriteLine($"\nwhat text color would you like to use");
        void ShowColor(ConsoleColor color, int number)
        {
            Console.ForegroundColor = color;
            Console.Write($"{color,-12}");
            Console.ResetColor();
            Console.WriteLine($": {number}");
        }

        ShowColor(ConsoleColor.Gray, 1);
        ShowColor(ConsoleColor.Black, 2);
        ShowColor(ConsoleColor.DarkBlue, 3);
        ShowColor(ConsoleColor.DarkGreen, 4);
        ShowColor(ConsoleColor.DarkCyan, 5);
        ShowColor(ConsoleColor.DarkRed, 6);
        ShowColor(ConsoleColor.DarkMagenta, 7);
        ShowColor(ConsoleColor.DarkYellow, 8);
        ShowColor(ConsoleColor.DarkGray, 9);
        ShowColor(ConsoleColor.Blue, 10);
        ShowColor(ConsoleColor.Green, 11);
        ShowColor(ConsoleColor.Cyan, 12);
        ShowColor(ConsoleColor.Red, 13);
        ShowColor(ConsoleColor.Magenta, 14);
        ShowColor(ConsoleColor.Yellow, 15);
        ShowColor(ConsoleColor.White, 16);



        int.TryParse(Console.ReadLine(), out int result);
        if(result == null || result > 16||result < 1)
        {
            Console.WriteLine("thats not a valid number");
            ChangeTextColor();
            return;
        }
        ConsoleColor[] colors =
        {
        ConsoleColor.Gray, ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen,
        ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,
        ConsoleColor.DarkGray, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan,
        ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White
        };
        Console.ForegroundColor = colors[result - 1];

        Program.MainMenu();
    }



}


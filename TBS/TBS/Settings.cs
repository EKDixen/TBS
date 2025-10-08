using Game.Class;

public class Settings
{
    public Settings() { }



    public void ChangeTextColor()
    {
        Console.WriteLine($"\nwhat text color would you like to use \nGray : 1, \nBlack : 2, \nDarkBlue : 3, \nDarkGreen : 4, \nDarkCyan : 5, \nDarkRed : 6, \nDarkMagenta : 7, \nDarkYellow : 8, \nDarkGray : 9," +
            $" \nBlue : 10, \nGreen : 11, \nCyan : 12, \nRed : 13, \nMagenta : 14, \nYellow : 15, \nWhite : 16");

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

        //Program.MainMenu();
    }



}


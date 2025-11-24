using System;
using System.Collections.Generic;

public class PlayerCreator
{
    private const int MaxUsernameLength = 12;
    private const int MaxPasswordLength = 12;

    public List<Player> players = new List<Player>();

    public Player PlayerCreatorFunction(PlayerDatabase db, string name)
    {
        string password;
        bool isValid;

        Console.WriteLine("--- Character Password Setup ---");
        Console.WriteLine($"Password Rules: Max {MaxPasswordLength} characters, no spaces.");

        do
        {
            Console.WriteLine("\nWrite a password for your character (needed to login):");
            password = Console.ReadLine();

            isValid = (password.Length <= MaxPasswordLength && !password.Contains(' '));

            if (!isValid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (password.Length > MaxPasswordLength)
                {
                    Console.WriteLine($"Error: Password is too long. Must be {MaxPasswordLength} characters or less.");
                }
                else if (password.Contains(' '))
                {
                    Console.WriteLine("Error: Password cannot contain spaces.");
                }
                else
                {
                    Console.WriteLine("Error: Invalid password format. Please try again.");
                }
                Console.ResetColor();
            }

        } while (!isValid);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Password accepted!");
        Console.ResetColor();

        //name, password, class, level, exp, HP, maxHP, Speed, armor, Dodge, DodgeNegation, critChance, critDamage, Stun, Stunnegation, location, money, luck
        Player newPlayer = new Player(name, password, ClassLibrary.Pathfinder, 1, 0, 100, 100, 10, 0, 5, 5, 5, 100, 5, 5, null, 10, 100);

        AttackManager atkManager = new AttackManager(newPlayer);
        atkManager.LearnAttack(AttackLibrary.ThrowHands);
        atkManager.EquipAttack(AttackLibrary.ThrowHands, 1);

        players.Add(newPlayer);
        return newPlayer;
    }
}
using Game.Class;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public enum SubLocationType
{
    shop,
    tavern,
    blacksmith, 
    arena,
    bank,
    casino



}
public class SubLocation
{
    public SubLocationType type;
    public string name;


    public List<(Item item,int quantity)> shopItems = new List<(Item,int)>();

    List<string> suits = new List<string>
    {
        new string("hearts"),
        new string("diamonds"),
        new string("clubs"),
        new string("spades")
    };

    public List<Item> bankItems = new List<Item>();
    public int bankMoney = 0;

    public int casinoMaxBet = 0;

    public SubLocation() { } //Deserialize

    public SubLocation(string tName,SubLocationType tType)
    {
        name = tName;
        type = tType;
    }


    
    public void DoSubLocation()
    {
        if (type == SubLocationType.shop)
        {
            ShopLogic();
        }
        if (type == SubLocationType.bank)
        {
            BankLogic();
        }
        if (type == SubLocationType.casino)
        {
            Console.WriteLine("what game do you want to play, \nBlackjack : 1 \nRoulette : 2 \nor leave : 0");
            int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0)
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
            else if (input == 1)
            {
                Console.Clear();
                BlackjackLogic();

            }
            else if (input == 2)
            {
                Console.Clear();
                RouletteLogic();
            }

        }


        // not done
        if (type == SubLocationType.tavern)
        {
            Console.WriteLine("do you want to buy something : 1  \nor do you want to rent a room : 2  \nor leave : 0");
            int.TryParse(Console.ReadLine(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if(input == 1)
            {
                ShopLogic();
            }
            else if (input == 2)
            {

            }


        }




    }


    //shop ----------------------------------
    void ShopLogic()
    {
        Inventory inventory = new Inventory(Program.player);



        Console.WriteLine("\n nr     Name                      Qty   Description        Price");
        Console.WriteLine(" ----------------------------------------------------------------");
        int i = 0;
        foreach (var item in shopItems)
        {
            i++;
            Console.WriteLine($" {i,-7}{item.item.name,-25} {item.quantity,-5} {item.item.description,-20} {item.item.value * item.quantity}");
        }
        Console.WriteLine("\nif you want to interact with anything type its corresponding number \nif not type 0");
        var n = int.TryParse(Console.ReadLine(), out int input);
        if (input == null || input > shopItems.Count || input < 0)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input == 0) { Program.MainMenu(); return; }
        input--;
        Console.WriteLine($"\nyou've picked {shopItems[input].item.name}\nit costs {shopItems[input].item.value * shopItems[input].quantity}\nyou have {Program.player.money} money");

        Console.WriteLine("0 : details");
        Console.WriteLine("1 : buy");
        Console.WriteLine("\ntype out the number next to the action you want to perform");

        var k = int.TryParse(Console.ReadLine(), out int ik);
        if (ik == null || ik < 0 || ik > 1)
        {
            Console.Clear();
            Console.WriteLine("my love would you please type a functional number this time\n ");
            DoSubLocation();
            return;
        }
        else if (ik == 0)
        {
            Console.Clear();
            Console.WriteLine($"\nyou've picked {shopItems[input].item.name}");
            Console.WriteLine($"{shopItems[input].item.details}\n");
        }
        else if (ik == 1)
        {
            if ((Program.player.money - shopItems[input].item.value * shopItems[input].quantity) >= 0)
            {
                inventory.AddItem(shopItems[input].item);
                Program.player.money -= shopItems[input].item.value * shopItems[input].quantity;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("you dont have enough money for that");
                Program.MainMenu();
                return;
            }

        }
        Program.SavePlayer();
        Program.MainMenu();
    }

    //bank ----------------------------------
    void BankLogic()
    {
        Inventory inventory = new Inventory(Program.player);

        Console.WriteLine($"\n\n\nyou have {bankMoney} money stored here and here is all the items you have stored here:");
        Console.WriteLine("\n nr     Name            Qty   Description        value");
        Console.WriteLine(" ------------------------------------------------------");
        int i = 1;
        int extraMoneyNumber = 0;
        if (bankMoney != 0)
        {
            Console.WriteLine($" {i,-7}{"money",-15} {bankMoney,-5}");
            extraMoneyNumber = 1;
        }

        foreach (var item in bankItems)
        {
            i++;
            Console.WriteLine($" {i,-7}{item.name,-15} {item.amount,-5} {item.description,-20} {item.value}");
        }
        Console.WriteLine("\nif you want to grab something type its nr \nif you want to deposit something or leave then type 0");
        var n = int.TryParse(Console.ReadLine(), out int input);
        if (input == null || input > bankItems.Count + extraMoneyNumber || input < 0)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input == 0)
        {
            Console.WriteLine("do you want to leave type 0, if you want to diposit type 1");
            var n2 = int.TryParse(Console.ReadLine(), out int input2);
            if (input2 == null || input2 > 1 || input2 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input2 == 0) { Program.MainMenu(); return; }
            else
            {
                Console.WriteLine($"\n\nyou have {Program.player.money} money\n\nand these are your items");

                Console.WriteLine("\nnr     Name            Qty   Description     value");
                Console.WriteLine("--------------------------------------------------");
                int id = 0;
                foreach (var item in Program.player.ownedItems)
                {
                    id++;
                    Console.WriteLine($"{id,-7}{item.name,-15} {item.amount,-5} {item.description,-16} {item.value}");
                }
                Console.WriteLine("\ntype out the nr of the item you want to diposit or type 0 if you want to deposit money");
                int.TryParse(Console.ReadLine(), out int input3);
                if (input3 == null || input3 > Program.player.ownedItems.Count || input3 < 0)
                {
                    Console.Clear();
                    Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                    DoSubLocation();
                    return;
                }
                if (input3 == 0)
                {
                    Console.WriteLine($"how much would you like to deposit? you have {Program.player.money}");
                    int.TryParse(Console.ReadLine(), out int moneyAmount);
                    if (moneyAmount == null || moneyAmount > Program.player.money || moneyAmount < 0)
                    {
                        Console.Clear();
                        Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                        DoSubLocation();
                        return;
                    }
                    Program.player.money -= moneyAmount;
                    bankMoney += moneyAmount;

                }
                else
                {
                    input3--;

                    bankItems.Add(Program.player.ownedItems[input3]);
                    inventory.DropItem(Program.player.ownedItems[input3]);
                }
            }
        }
        else if (input == 1)
        {

            Console.WriteLine($"how much would you like to grab? theres {bankMoney} stored");
            int.TryParse(Console.ReadLine(), out int moneyAmount);
            if (moneyAmount == null || moneyAmount > bankMoney || moneyAmount < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            Program.player.money += moneyAmount;
            bankMoney -= moneyAmount;

        }
        else
        {
            bankItems.Remove(Program.player.ownedItems[input]);
            inventory.AddItem(Program.player.ownedItems[input]);
        }
        Program.SavePlayer();
        Program.MainMenu();
    }

    //casino --------------------------------
    void BlackjackLogic()
    {
        Console.WriteLine($"\nhow much do you want to bet?  current cash: {Program.player.money} \nthe max bet is {casinoMaxBet}");
        int.TryParse(Console.ReadLine(), out int bet);
        if (bet == null || bet > casinoMaxBet || bet < 0)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (bet > Program.player.money)
        {
            Console.WriteLine("\nyou dont have that much money\n ");
            DoSubLocation();
            return;
        }
        Console.WriteLine($"you bet: {bet}");
        Program.player.money -= bet;

        Random rand = new Random();
        int dealer1 = rand.Next(1, 14);
        int dealer1Suit = rand.Next(1, 4);

        Console.Write($"\n\ndealer shows a");
        if (dealer1 == 1) Console.Write($"n ace of {suits[dealer1Suit]} (worth 11 and 1)");
        else if (dealer1 < 11) Console.Write($" {dealer1} of {suits[dealer1Suit]}");
        else Console.Write($" 10 of {suits[dealer1Suit]}");


        int player1 = rand.Next(1, 14);
        int player1Suit = rand.Next(1, 4);

        int player2 = rand.Next(1, 14);
        int player2Suit = rand.Next(1, 4);

        Console.Write($"\n\nyou have a");
        if (player1 == 1) Console.Write($"n ace of {suits[player1Suit]} (worth 11 and 1)");
        else if (player1 < 11) Console.Write($" {player1} of {suits[player1Suit]}");
        else Console.Write($" 10 of {suits[player1Suit]}");

        Console.Write($"\nand a");
        if (player2 == 1) Console.Write($"n ace of {suits[player2Suit]} (worth 11 and 1)");
        else if (player2 < 11) Console.Write($" {player2} of {suits[player2Suit]}");
        else Console.Write($" 10 of {suits[player2Suit]}");

        int playerValue = 0;
        int dealerValue = 0;
        if (player1 > 10) playerValue += 10;
        else if (player1 == 1 && playerValue + 11 <= 21) playerValue += 11;
        else { playerValue += player1; player1 = 0; }

        if (player2 > 10) playerValue += 10;
        else if (player2 == 1 && playerValue + 11 <= 21) playerValue += 11;
        else { playerValue += player2; player2 = 0; }

        int player3 = 0;

        Console.WriteLine($"\nyour card sum: {playerValue}");

        while (true)
        {
            if (playerValue > 21)
            {
                if (player1 == 1) { playerValue -= 10; player1 = 0; continue; }
                if (player2 == 1) { playerValue -= 10; player2 = 0; continue; }
                if (player3 == 1) { playerValue -= 10; player3 = 0; continue; }
                Console.WriteLine("\nyou bust");
                break;
            }
            else if (playerValue == 21)
            {
                break;
            }

            Console.WriteLine("\ndo you wish to hit : 1\nor stand : 2");

            int.TryParse(Console.ReadLine(), out int input2);
            if (input2 == null || input2 > 2 || input2 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input2 == 1)
            {
                player3 = rand.Next(1, 14);
                int player3Suit = rand.Next(1, 4);

                Console.Write($"\nyou get a");
                if (player3 == 1) Console.Write($"n ace of {suits[player3Suit]} (worth 11 and 1)");
                else if (player3 < 11) Console.Write($" {player3} of {suits[player3Suit]}");
                else Console.Write($" 10 of {suits[player3Suit]}");

                if (player3 > 10) playerValue += 10;
                else if (player3 == 1 && playerValue + 11 <= 21) playerValue += 11;
                else { playerValue += player3; player3 = 0; }

                Console.WriteLine($"\nyour card sum: {playerValue}");
                continue;
            }
            else if (input2 == 2)
            {
                int dealer2 = rand.Next(1, 14);
                int dealer2Suit = rand.Next(1, 4);

                Thread.Sleep(200);

                Console.Write($"\ndealer shows a");
                if (dealer2 == 1) Console.Write($"n ace of {suits[dealer2Suit]} (worth 11 and 1)");
                else if (dealer2 < 11) Console.Write($" {dealer2} of {suits[dealer2Suit]}");
                else Console.Write($" 10 of {suits[dealer2Suit]}");

                if (dealer1 > 10) dealerValue += 10;
                else if (dealer1 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                else { dealerValue += dealer1; dealer1 = 0; }

                if (dealer2 > 10) dealerValue += 10;
                else if (dealer2 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                else { dealerValue += dealer2; dealer2 = 0; }

                Thread.Sleep(200);

                Console.WriteLine($"\ndealer card sum: {dealerValue}");
                while (dealerValue < 17)
                {
                    

                    int dealer3 = rand.Next(1, 14);
                    int dealer3Suit = rand.Next(1, 4);

                    Thread.Sleep(200);

                    Console.Write($"\ndealer shows a");
                    if (dealer3 == 1) Console.Write($"n ace of {suits[dealer3Suit]} (worth 11 and 1)");
                    else if (dealer3 < 11) Console.Write($" {dealer3} of {suits[dealer3Suit]}");
                    else Console.Write($" 10 of {suits[dealer3Suit]}");

                    if (dealer3 > 10) dealerValue += 10;
                    else if (dealer3 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                    else { dealerValue += dealer3; dealer3 = 0; }

                    if (dealerValue > 21)
                    {
                        if (dealer1 == 1) { dealerValue -= 10; dealer1 = 0; continue; }
                        if (dealer2 == 1) { dealerValue -= 10; dealer2 = 0; continue; }
                        if (dealer3 == 1) { dealerValue -= 10; dealer3 = 0; continue; }
                    }
                    Thread.Sleep(200);
                    Console.WriteLine($"\ndealer card sum: {dealerValue}");
                }

                break;
            }
        }
        if (playerValue > 21)
        {
            Console.WriteLine($"\nyou lose your bet of {bet} cash");
            Program.SavePlayer();

            Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadLine(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                Console.Clear();
                DoSubLocation();
                return;
            }
            else
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
        }
        else if (dealerValue > 21)
        {
            Program.player.money += bet * 2;
            Console.WriteLine($"\nyou win your bet of {bet} cash");
            Program.SavePlayer();

            Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadLine(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                Console.Clear();
                DoSubLocation();
                return;
            }
            else
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue > dealerValue)
        {
            Program.player.money += bet * 2;

            Console.WriteLine($"\nyou win your bet of {bet} cash");
            Program.SavePlayer();

            Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadLine(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                Console.Clear();
                DoSubLocation();
                return;
            }
            else
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue < dealerValue)
        {
            Console.WriteLine($"\nyou lose your bet of {bet} cash");
            Program.SavePlayer();

            Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadLine(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                Console.Clear();
                DoSubLocation();
                return;
            }
            else
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue == dealerValue)
        {
            Program.player.money += bet;
            Console.WriteLine($"\nyou draw");

            Program.SavePlayer();

            Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadLine(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                Console.Clear();
                Console.WriteLine("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                Console.Clear();
                DoSubLocation();
                return;
            }
            else
            {
                Console.Clear();
                Program.MainMenu();
                return;
            }
        }
    }
    void RouletteLogic()
    {
        Console.WriteLine($"\nhow much do you want to bet?  current cash: {Program.player.money} \nthe max bet is {casinoMaxBet}");
        int.TryParse(Console.ReadLine(), out int bet);
        if (bet == null || bet > casinoMaxBet || bet < 0)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (bet > Program.player.money)
        {
            Console.WriteLine("\nyou dont have that much money\n ");
            DoSubLocation();
            return;
        }
        Console.WriteLine($"you bet: {bet}\n");
        Program.player.money -= bet;

        Console.WriteLine($"where would you like to bet, \nblack : 1 \nred : 2 \neven : 3 \nodd : 4 \n1st 12 : 5 " +
            $"\n2nd 12 : 6 \n3rd 12 : 7 \n1 to 18(half) : 8 \n19 to 36(half) : 9 \nspecific number : 10\n");


        Random rand = new Random();
        int result = rand.Next(0, 35);

        HashSet<int> red = new HashSet<int>
                {
                    1, 3, 5, 7, 9, 12, 14, 16, 18,
                    19, 21, 23, 25, 27, 30, 32, 34, 36
                };

        int.TryParse(Console.ReadLine(), out int betPlace);
        switch (betPlace)
        {
            case 1:
                Console.WriteLine($"you bet black \nthe number rolled is {result}");

                if (result == 0) ;
                else if (!red.Contains(result) && result != 0)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 2:
                Console.WriteLine($"you bet red \nthe number rolled is {result}");

                if (result == 0) ;
                else if (red.Contains(result))
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 3:
                Console.WriteLine($"you bet even \nthe number rolled is {result}");
                if (result % 2 == 0 && result != 0)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 4:
                Console.WriteLine($"you bet odd \nthe number rolled is {result}");
                if (result % 2 == 1)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    Console.WriteLine("you lose\n");
                }

                break;
            case 5:
                Console.WriteLine($"you bet 1st 12 \nthe number rolled is {result}");
                if (result <= 12 && result != 0)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 6:
                Console.WriteLine($"you bet 2nd 12 \nthe number rolled is {result}");
                if (result > 12 && result <= 24)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 7:
                Console.WriteLine($"you bet 3rd 12 \nthe number rolled is {result}");
                if (result > 24)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 8:
                Console.WriteLine($"you bet 1st 18(half) \nthe number rolled is {result}");
                if (result <= 18 && result != 0)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 9:
                Console.WriteLine($"you bet 2nd 18(half) \nthe number rolled is {result}");
                if (result > 18)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;
            case 10:
                int betNumber;
                while (true)
                {
                    Console.WriteLine("what number?");
                    int.TryParse(Console.ReadLine(), out betNumber);
                    if (betNumber != null && betNumber < 36)
                    {
                        break;
                    }
                    Console.WriteLine("type a number please");
                }

                Console.WriteLine($"you bet {betNumber} \nthe number rolled is {result}");
                if (result == betNumber)
                {
                    Console.WriteLine("you win!!!");
                    Program.player.money += bet * 34;
                }
                else
                {
                    Console.WriteLine("you lose\n");
                }
                break;

        }

        Console.WriteLine($"you have {Program.player.money} money");

        Console.WriteLine("\ndo you wish to play again : 1\n or leave : 0");
        int.TryParse(Console.ReadLine(), out int input3);
        if (input3 == null || input3 > 2 || input3 < 0)
        {
            Console.Clear();
            Console.WriteLine("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input3 == 1)
        {
            Console.Clear();
            DoSubLocation();
            return;
        }
        else
        {
            Console.Clear();
            Program.MainMenu();
            return;
        }



    
    }



}


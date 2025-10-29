using Game.Class;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public enum SubLocationType
{
    shop,//done
    tavern,//no--
    blacksmith,//no--
    arena,//no--
    bank,//done
    casino,//done
    wilderness,//done
    graveyard,//no--
    pond//done--
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
        MainUI.ClearMainArea();
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
            MainUI.WriteInMainArea("what game do you want to play, \nBlackjack : 1 \nRoulette : 2 \nor leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input == 0)
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
            else if (input == 1)
            {
                MainUI.ClearMainArea();
                BlackjackLogic();

            }
            else if (input == 2)
            {
                MainUI.ClearMainArea();
                RouletteLogic();
            }

        }
        if (type == SubLocationType.wilderness)
        {
            WildernessLogic();
        }
        if (type == SubLocationType.pond)
        {
            FishingLogic();
        }



        // not done---
        if (type == SubLocationType.tavern)
        {
            MainUI.WriteInMainArea("do you want to buy something : 1  \nor do you want to rent a room : 2  \nor leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input);
            if (input == null || input > 2 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
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


    #region shop
    void ShopLogic()
    {
        Inventory inventory = new Inventory(Program.player);


        MainUI.WriteInMainArea("Shop Items:");
        MainUI.WriteInMainArea("\n nr     Name                      Qty   Description        Price");
        MainUI.WriteInMainArea(" ----------------------------------------------------------------");
        int i = 0;
        foreach (var item in shopItems)
        {
            i++;
            MainUI.WriteInMainArea($" {i,-7}{item.item.name,-25} {item.quantity,-5} {item.item.description,-20} {item.item.value * item.quantity}");
        }
        MainUI.WriteInMainArea("\nif you want to interact with anything type its corresponding number \nif not type 0");
        var n = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input);
        if (input == null || input > shopItems.Count || input < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input == 0) { Program.MainMenu(); return; }
        input--;
        MainUI.WriteInMainArea($"\nyou've picked {shopItems[input].item.name}\nit costs {shopItems[input].item.value * shopItems[input].quantity}\nyou have {Program.player.money} money");

        MainUI.WriteInMainArea("0 : details");
        MainUI.WriteInMainArea("1 : buy");
        MainUI.WriteInMainArea("\ntype out the number next to the action you want to perform");

        var k = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int ik);
        if (ik == null || ik < 0 || ik > 1)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("my love would you please type a functional number this time\n ");
            DoSubLocation();
            return;
        }
        else if (ik == 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"\nyou've picked {shopItems[input].item.name}");
            MainUI.WriteInMainArea($"{shopItems[input].item.details}\n");

            MainUI.WriteInMainArea($"-press Enter to continue");
            Console.ReadLine();
        }
        else if (ik == 1)
        {
            if ((Program.player.money - shopItems[input].item.value * shopItems[input].quantity) >= 0)
            {
                inventory.AddItem(shopItems[input].item, shopItems[input].quantity);
                Program.player.money -= shopItems[input].item.value * shopItems[input].quantity;
            }
            else
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("\nyou dont have enough money for that");
                Thread.Sleep(1000);
                DoSubLocation();
                return;
            }

        }
        Program.SavePlayer();
        DoSubLocation();
    }
    #endregion

    #region bank
    void BankLogic()
    {
        Inventory inventory = new Inventory(Program.player);

        MainUI.WriteInMainArea($"\n\n\nyou have {bankMoney} money stored here and here is all the items you have stored here:");
        MainUI.WriteInMainArea("\n nr     Name            Qty   Description        value");
        MainUI.WriteInMainArea(" ------------------------------------------------------");
        int i = 1;
        int extraMoneyNumber = 0;
        if (bankMoney != 0)
        {
            MainUI.WriteInMainArea($" {i,-7}{"money",-15} {bankMoney,-5}");
            extraMoneyNumber = 1;
        }

        foreach (var item in bankItems)
        {
            i++;
            MainUI.WriteInMainArea($" {i,-7}{item.name,-15} {item.amount,-5} {item.description,-20} {item.value}");
        }
        MainUI.WriteInMainArea("\nif you want to grab something type its nr \nif you want to deposit something or leave then type 0");
        var n = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input);
        if (input == null || input > bankItems.Count + extraMoneyNumber || input < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input == 0)
        {
            MainUI.WriteInMainArea("do you want to leave type 0, if you want to diposit type 1");
            var n2 = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input2);
            if (input2 == null || input2 > 1 || input2 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input2 == 0) { Program.MainMenu(); return; }
            else
            {
                MainUI.WriteInMainArea($"\n\nyou have {Program.player.money} money\n\nand these are your items");

                MainUI.WriteInMainArea("\nnr     Name            Qty   Description     value");
                MainUI.WriteInMainArea("--------------------------------------------------");
                int id = 0;
                foreach (var item in Program.player.ownedItems)
                {
                    id++;
                    MainUI.WriteInMainArea($"{id,-7}{item.name,-15} {item.amount,-5} {item.description,-16} {item.value}");
                }
                MainUI.WriteInMainArea("\ntype out the nr of the item you want to diposit or type 0 if you want to deposit money");
                int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
                if (input3 == null || input3 > Program.player.ownedItems.Count || input3 < 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                    DoSubLocation();
                    return;
                }
                if (input3 == 0)
                {
                    MainUI.WriteInMainArea($"how much would you like to deposit? you have {Program.player.money}");
                    int.TryParse(Console.ReadLine(), out int moneyAmount);
                    if (moneyAmount == null || moneyAmount > Program.player.money || moneyAmount < 0)
                    {
                        MainUI.ClearMainArea();
                        MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
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

            MainUI.WriteInMainArea($"how much would you like to grab? theres {bankMoney} stored");
            int.TryParse(Console.ReadLine(), out int moneyAmount);
            if (moneyAmount == null || moneyAmount > bankMoney || moneyAmount < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            Program.player.money += moneyAmount;
            bankMoney -= moneyAmount;

        }
        else
        {
            bankItems.Remove(Program.player.ownedItems[input]);
            inventory.AddItem(Program.player.ownedItems[input], Program.player.ownedItems[input].amount);
        }
        Program.SavePlayer();
        Program.MainMenu();
    }
    #endregion

    #region casino
    void BlackjackLogic()
    {
        MainUI.WriteInMainArea($"how much do you want to bet?  current cash: {Program.player.money} \nthe max bet is {casinoMaxBet}");
        int.TryParse(Console.ReadLine(), out int bet);
        if (bet == null || bet > casinoMaxBet || bet < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (bet > Program.player.money)
        {
            MainUI.WriteInMainArea("\nyou dont have that much money\n ");
            DoSubLocation();
            return;
        }
        MainUI.WriteInMainArea($"you bet: {bet}");
        Program.player.money -= bet;
        Program.SavePlayer();

        Random rand = new Random();
        int dealer1 = rand.Next(1, 14);
        int dealer1Suit = rand.Next(1, 4);


        string dealString = "\ndealer shows a";
        
        if (dealer1 == 1) dealString += $"n ace of {suits[dealer1Suit]} (worth 11 and 1)";
        else if (dealer1 < 11) dealString += $" {dealer1} of {suits[dealer1Suit]}";
        else dealString += $" 10 of {suits[dealer1Suit]}";

        MainUI.WriteInMainArea(dealString);


        int player1 = rand.Next(1, 14);
        int player1Suit = rand.Next(1, 4);

        int player2 = rand.Next(1, 14);
        int player2Suit = rand.Next(1, 4);

        string playString = $"\nyou have a";

        if (player1 == 1) playString += $"n ace of {suits[player1Suit]} (worth 11 and 1)";
        else if (player1 < 11) playString +=($" {player1} of {suits[player1Suit]}");
        else playString +=($" 10 of {suits[player1Suit]}");

        MainUI.WriteInMainArea(playString);

        playString =$"and a";
        if (player2 == 1) playString += $"n ace of {suits[player2Suit]} (worth 11 and 1)";
        else if (player2 < 11) playString += ($" {player2} of {suits[player2Suit]}");
        else playString += ($" 10 of {suits[player2Suit]}");

        MainUI.WriteInMainArea(playString);

        int playerValue = 0;
        int dealerValue = 0;
        if (player1 > 10) playerValue += 10;
        else if (player1 == 1 && playerValue + 11 <= 21) playerValue += 11;
        else { playerValue += player1; player1 = 0; }

        if (player2 > 10) playerValue += 10;
        else if (player2 == 1 && playerValue + 11 <= 21) playerValue += 11;
        else { playerValue += player2; player2 = 0; }

        int player3 = 0;

        MainUI.WriteInMainArea($"your card sum: {playerValue}");

        bool doTurns = true;
        if (playerValue == 21)
        {
            int dealer2 = rand.Next(1, 14);
            int dealer2Suit = rand.Next(1, 4);

            Thread.Sleep(700);

            dealString = ($"\ndealer shows a");
            if (dealer2 == 1) dealString += ($"n ace of {suits[dealer2Suit]} (worth 11 and 1)");
            else if (dealer2 < 11) dealString += ($" {dealer2} of {suits[dealer2Suit]}");
            else dealString += ($" 10 of {suits[dealer2Suit]}");

            MainUI.WriteInMainArea(dealString);


            if (dealer2 > 10) dealerValue += 10;
            else if (dealer2 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
            else { dealerValue += dealer2; dealer2 = 0; }

            MainUI.WriteInMainArea($"dealer card sum: {dealerValue}");
            
            doTurns = false;

        }
        while (doTurns)
        {
            if (playerValue > 21)
            {
                if (player1 == 1) { playerValue -= 10; player1 = 0; continue; }
                if (player2 == 1) { playerValue -= 10; player2 = 0; continue; }
                if (player3 == 1) { playerValue -= 10; player3 = 0; continue; }
                MainUI.WriteInMainArea("\nyou bust");
                break;
            }

            MainUI.WriteInMainArea("\ndo you wish to hit : 1\nor stand : 2");

            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input2);
            if (input2 == null || input2 > 2 || input2 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input2 == 1)
            {
                player3 = rand.Next(1, 14);
                int player3Suit = rand.Next(1, 4);

                playString=($"\nyou get a");
                if (player3 == 1) playString += ($"n ace of {suits[player3Suit]} (worth 11 and 1)");
                else if (player3 < 11) playString += ($" {player3} of {suits[player3Suit]}");
                else playString += ($" 10 of {suits[player3Suit]}");

                MainUI.WriteInMainArea(playString);

                if (player3 > 10) playerValue += 10;
                else if (player3 == 1 && playerValue + 11 <= 21) playerValue += 11;
                else { playerValue += player3; player3 = 0; }

                MainUI.WriteInMainArea($"your card sum: {playerValue}");
                continue;
            }
            else if (input2 == 2)
            {
                int dealer2 = rand.Next(1, 14);
                int dealer2Suit = rand.Next(1, 4);

                Thread.Sleep(700);

                dealString = ($"\ndealer shows a");
                if (dealer2 == 1) dealString += ($"n ace of {suits[dealer2Suit]} (worth 11 and 1)");
                else if (dealer2 < 11) dealString += ($" {dealer2} of {suits[dealer2Suit]}");
                else dealString += ($" 10 of {suits[dealer2Suit]}");

                MainUI.WriteInMainArea(dealString);

                if (dealer1 > 10) dealerValue += 10;
                else if (dealer1 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                else { dealerValue += dealer1; dealer1 = 0; }

                if (dealer2 > 10) dealerValue += 10;
                else if (dealer2 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                else { dealerValue += dealer2; dealer2 = 0; }

                Thread.Sleep(700);

                MainUI.WriteInMainArea($"dealer card sum: {dealerValue}");
                while (dealerValue < 17)
                {
                    

                    int dealer3 = rand.Next(1, 14);
                    int dealer3Suit = rand.Next(1, 4);

                    Thread.Sleep(700);

                    dealString=($"\ndealer shows a");
                    if (dealer3 == 1) dealString+=($"n ace of {suits[dealer3Suit]} (worth 11 and 1)");
                    else if (dealer3 < 11) dealString+=($" {dealer3} of {suits[dealer3Suit]}");
                    else dealString+=($" 10 of {suits[dealer3Suit]}");

                    MainUI.WriteInMainArea(dealString);

                    if (dealer3 > 10) dealerValue += 10;
                    else if (dealer3 == 1 && dealerValue + 11 <= 21) dealerValue += 11;
                    else { dealerValue += dealer3; dealer3 = 0; }

                    if (dealerValue > 21)
                    {
                        if (dealer1 == 1) { dealerValue -= 10; dealer1 = 0; continue; }
                        if (dealer2 == 1) { dealerValue -= 10; dealer2 = 0; continue; }
                        if (dealer3 == 1) { dealerValue -= 10; dealer3 = 0; continue; }
                    }
                    Thread.Sleep(500);
                    MainUI.WriteInMainArea($"dealer card sum: {dealerValue}");
                }

                break;
            }
        }
        if (playerValue > 21)
        {
            MainUI.WriteInMainArea($"\nyou lose your bet of {bet} cash");
            Program.SavePlayer();

            MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                MainUI.ClearMainArea();
                DoSubLocation();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
        }
        else if (dealerValue > 21)
        {
            Program.player.money += bet * 2;
            MainUI.WriteInMainArea($"\nyou win your bet of {bet} cash");
            Program.SavePlayer();

            MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                MainUI.ClearMainArea();
                DoSubLocation();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue > dealerValue)
        {
            Program.player.money += bet * 2;

            MainUI.WriteInMainArea($"\nyou win your bet of {bet} cash");
            Program.SavePlayer();

            MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                MainUI.ClearMainArea();
                DoSubLocation();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue < dealerValue)
        {
            MainUI.WriteInMainArea($"\nyou lose your bet of {bet} cash");
            Program.SavePlayer();

            MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                MainUI.ClearMainArea();
                DoSubLocation();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
        }
        else if (playerValue == dealerValue)
        {
            Program.player.money += bet;
            MainUI.WriteInMainArea($"\nyou draw");

            Program.SavePlayer();

            MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
            if (input3 == null || input3 > 2 || input3 < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                DoSubLocation();
                return;
            }
            else if (input3 == 1)
            {
                MainUI.ClearMainArea();
                DoSubLocation();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                Program.MainMenu();
                return;
            }
        }
    }
    void RouletteLogic()
    {
        MainUI.WriteInMainArea($"\nhow much do you want to bet?  current cash: {Program.player.money} \nthe max bet is {casinoMaxBet}");
        int.TryParse(Console.ReadLine(), out int bet);
        if (bet == null || bet > casinoMaxBet || bet < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (bet > Program.player.money)
        {
            MainUI.WriteInMainArea("\nyou dont have that much money\n ");
            DoSubLocation();
            return;
        }
        MainUI.WriteInMainArea($"you bet: {bet}\n");
        Program.player.money -= bet;
        Program.SavePlayer();

        MainUI.WriteInMainArea($"where would you like to bet, \nblack : 1 \nred : 2 \neven : 3 \nodd : 4 \n1st 12 : 5 " +
            $"\n2nd 12 : 6 \n3rd 12 : 7 \n1 to 18(half) : 8 \n19 to 36(half) : 9 \nspecific number : 10\n");


        Random rand = new Random();
        int result = rand.Next(0, 35);

        HashSet<int> red = new HashSet<int>
                {
                    1, 3, 5, 7, 9, 12, 14, 16, 18,
                    19, 21, 23, 25, 27, 30, 32, 34, 36
                };

        int.TryParse(Console.ReadKey().KeyChar.ToString(), out int betPlace);
        switch (betPlace)
        {
            case 1:
                MainUI.WriteInMainArea($"you bet black \nthe number rolled is {result}");

                if (result == 0) ;
                else if (!red.Contains(result) && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 2:
                MainUI.WriteInMainArea($"you bet red \nthe number rolled is {result}");

                if (result == 0) ;
                else if (red.Contains(result))
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 3:
                MainUI.WriteInMainArea($"you bet even \nthe number rolled is {result}");
                if (result % 2 == 0 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 4:
                MainUI.WriteInMainArea($"you bet odd \nthe number rolled is {result}");
                if (result % 2 == 1)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;

                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }

                break;
            case 5:
                MainUI.WriteInMainArea($"you bet 1st 12 \nthe number rolled is {result}");
                if (result <= 12 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 6:
                MainUI.WriteInMainArea($"you bet 2nd 12 \nthe number rolled is {result}");
                if (result > 12 && result <= 24)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 7:
                MainUI.WriteInMainArea($"you bet 3rd 12 \nthe number rolled is {result}");
                if (result > 24)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 3;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 8:
                MainUI.WriteInMainArea($"you bet 1st 18(half) \nthe number rolled is {result}");
                if (result <= 18 && result != 0)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 9:
                MainUI.WriteInMainArea($"you bet 2nd 18(half) \nthe number rolled is {result}");
                if (result > 18)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 2;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;
            case 10:
                int betNumber;
                while (true)
                {
                    MainUI.WriteInMainArea("what number?");
                    int.TryParse(Console.ReadLine(), out betNumber);
                    if (betNumber != null && betNumber < 36)
                    {
                        break;
                    }
                    MainUI.WriteInMainArea("type a number please");
                }

                MainUI.WriteInMainArea($"you bet {betNumber} \nthe number rolled is {result}");
                if (result == betNumber)
                {
                    MainUI.WriteInMainArea("you win!!!");
                    Program.player.money += bet * 34;
                }
                else
                {
                    MainUI.WriteInMainArea("you lose\n");
                }
                break;

        }

        MainUI.WriteInMainArea($"you have {Program.player.money} money");

        MainUI.WriteInMainArea("\ndo you wish to play again : 1\n or leave : 0");
        int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input3);
        if (input3 == null || input3 > 2 || input3 < 0)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
            DoSubLocation();
            return;
        }
        else if (input3 == 1)
        {
            MainUI.ClearMainArea();
            DoSubLocation();
            return;
        }
        else
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }



    
    }
    #endregion

    #region wilderness
    void WildernessLogic()
    {
        EncounterManager encounterManager = new EncounterManager(Program.player);
        encounterManager.ProcessWildernessEncounters(LocationLibrary.Get(Program.player.currentLocation));

        Program.MainMenu();
    }


    #endregion

    #region fishingPond

    int fishingMeter=1;
    int fishingMeterTarget = 15;

    int position = 0;
    int direction = 1;
    int width = 10;         
    int target = 5;        
    bool fishing = true;   

    void FishingLogic()
    {
        fishingMeter = 1;

        MainUI.WriteInMainArea("You ready to start fishing?\npress Enter to start \nor 1 to go back to menu");
        string input = Console.ReadKey().KeyChar.ToString();
        int.TryParse(input, out int r);
        if (r==1)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        fishing = true;
        
        while (fishing) 
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (position == target)
                    {
                        MainUI.WriteInMainArea("PERFECT!");

                        fishingMeter += 3;
                    }
                    else if (position >= target - 1 && position <= target + 1)
                    {
                        MainUI.WriteInMainArea("Good! ");

                        fishingMeter++;
                    }
                    else
                    {
                        MainUI.WriteInMainArea("Oh no... it got away.");

                        MainUI.WriteInMainArea("-press Enter to continue");
                        Console.ReadLine();
                        fishing = false;
                        break;
                    }
                    
                    Thread.Sleep(200);
                }
            }

            position += direction;

            if (position >= width)
            {
                position = width;
                direction = -1; 
            }
            else if (position <= 0)
            {
                position = 0;
                direction = 1;  
            }



            MainUI.ClearMainArea();

            DrawFishingBar(fishingMeter, fishingMeterTarget, 30);

            MainUI.WriteInMainArea("press Enter when the x is over the v");

            string targetLine = new string('_', target) + "V" + new string('_', target);
            MainUI.WriteInMainArea(targetLine);

            string movingLine = new string('_', position) + "X" + new string('_', width - position);
            MainUI.WriteInMainArea(movingLine);

            if (fishingMeter >= fishingMeterTarget)
            {
                MainUI.WriteInMainArea("\nYOU GOT FISH!!");

                Inventory inv= new Inventory(Program.player);
                inv.AddItem(ItemLibrary.fish,1);

                MainUI.WriteInMainArea("-press Enter to continue");
                Console.ReadLine();
                fishing = false;
                break;
            }

            Thread.Sleep(100);

        }

        DoSubLocation();
    }
    private static void DrawFishingBar(int current, int max, int width)
    {
        int barWidth = width - 15;
        int filled = max > 0 ? (int)((double)current / max * barWidth) : 0;
        filled = Math.Max(0, Math.Min(filled, barWidth));

        string st = "Fish: "+ new string('█', filled)+ new string('░', barWidth - filled)+ $" {Math.Max(0, current)}/{max}";
        MainUI.WriteInMainArea(st);
    }

    #endregion


    //not done



}
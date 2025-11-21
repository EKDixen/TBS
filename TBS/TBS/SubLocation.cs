using Game.Class;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public enum SubLocationType
{
    shop,
    tavern,//no--
    blacksmith,//no--
    arena,//no--
    bank,
    casino,
    wilderness,
    graveyard,
    pond,
    port
}
public class SubLocation
{
    public SubLocationType type;
    public string name;


    public List<Item> shopItems = new List<Item>();

    List<string> suits = new List<string>
    {
        new string("hearts"),
        new string("diamonds"),
        new string("clubs"),
        new string("spades")
    };

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
        if (type == SubLocationType.graveyard)
        {
            GraveyardLogic();
        }
        if (type == SubLocationType.port)
        {
            PortLogic();
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
        MainUI.WriteInMainArea("\n nr     Name                      Weight   Description        Price");
        MainUI.WriteInMainArea(" ----------------------------------------------------------------");
        int i = 0;
        foreach (var item in shopItems)
        {
            i++;
            MainUI.WriteInMainArea($" {i,-7}{item.name,-26} {item.weight,-7} {item.description,-20} {item.value}");
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
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"you've picked {shopItems[input].name}   it costs {shopItems[input].value}\n");

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
            MainUI.WriteInMainArea($"\nyou've picked {shopItems[input].name}");
            MainUI.WriteInMainArea($"{shopItems[input].detailsLore}\n");

            MainUI.WriteInMainArea($"Press Enter to continue...");
            Console.ReadLine();
        }
        else if (ik == 1)
        {
            MainUI.WriteInMainArea($"how many would you like to buy?");
            var q = int.TryParse(Console.ReadLine(),out int quantity);
            if (quantity == null || quantity <= 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                DoSubLocation();
                return;
            }
            if ((Program.player.money - shopItems[input].value * quantity) >= 0)
            {
                inventory.AddItem(shopItems[input], quantity);
                Program.player.money -= shopItems[input].value * quantity;
            }
            else
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("\nyou dont have enough Rai for that");
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
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the bank");
            MainUI.WriteInMainArea($"You have {Program.player.bankMoney} Rai in your account.");
            MainUI.WriteInMainArea("Would you like to:");
            MainUI.WriteInMainArea("1. Deposit Items");
            MainUI.WriteInMainArea("2. Withdraw Items");
            MainUI.WriteInMainArea("3. Deposit Money");
            MainUI.WriteInMainArea("4. Withdraw Money");
            MainUI.WriteInMainArea("0. Leave");

            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out int input) == false || input > 4 || input < 0)
            {
                MainUI.WriteInMainArea(" \nyou gotta type a number from 0-4");
                Thread.Sleep(1000);
                continue;
            }

            switch (input)
            {
                case 0:
                    Program.MainMenu();
                    return;
                case 1:
                    DepositItems();
                    break;
                case 2:
                    WithdrawItems();
                    break;
                case 3:
                    DepositMoney();
                    break;
                case 4:
                    WithdrawMoney();
                    break;
            }
            Program.SavePlayer(); 
        }
    }

    private void DepositItems()
    {
        Inventory inventory = new Inventory(Program.player);

        const float exponent = 1.5f;
        const float scale = 0.1f;

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Select an item to deposit:");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty");
            MainUI.WriteInMainArea("--------------------------------------");

            if (Program.player.ownedItems.Count == 0)
            {
                MainUI.WriteInMainArea("You have no items to deposit.");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0. Back");
            }
            else
            {
                for (int i = 0; i < Program.player.ownedItems.Count; i++)
                {
                    var item = Program.player.ownedItems[i];
                    MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-5}");
                }
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0. Back");
            }

            string inputString = Console.ReadLine() ?? "";
            var n = int.TryParse(inputString, out int input);

            if (input == 0)
            {
                return; 
            }

            if (!n || input < 1 || input > Program.player.ownedItems.Count)
            {
                MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list.");
                Thread.Sleep(1000);
                continue;
            }

            Item selectedItem = Program.player.ownedItems[input - 1];
            int quantity = 1;

            // If stackable, ask how many
            if (selectedItem.type != ItemType.equipment)
            {
                MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to deposit? (Max: {selectedItem.amount})");
                string quantityString = Console.ReadLine() ?? "";
                var q = int.TryParse(quantityString, out quantity);

                if (!q || quantity < 1 || quantity > selectedItem.amount)
                {
                    MainUI.WriteInMainArea("\nInvalid amount.");
                    Thread.Sleep(1000);
                    continue;
                }
            }

            // Check if item is equipped
            int equippedSlot = Program.player.equippedItems.IndexOf(selectedItem);
            if (equippedSlot >= 0)
            {
                MainUI.WriteInMainArea($"\nThis item is equipped. Unequipping {selectedItem.name}...");
                inventory.UnequipItem(equippedSlot);
                Thread.Sleep(1000);
            }

            Item itemToBank = new Item(selectedItem); // Use copy constructor
            itemToBank.amount = quantity;

            // Add to bank list
            Program.player.bankItems.Add((Program.player.currentLocation, itemToBank));

            // Handle Stats & Effects
            if (selectedItem.type == ItemType.Artifact)
            {
                inventory.RemoveEffects(selectedItem, null); // Remove stats for the whole stack
            }

            // Handle Weight & Speed 
            float totalWeightRemoved = selectedItem.weight * quantity;

            Program.player.speed += (int)MathF.Floor(MathF.Pow(Program.player.inventorySpeedModifier * scale, exponent));
            Program.player.inventorySpeedModifier -= (selectedItem.weight - 20) * quantity;
            Program.player.speed -= (int)MathF.Floor(MathF.Pow(Program.player.inventorySpeedModifier * scale, exponent));
            Program.player.inventoryWeight -= totalWeightRemoved;


            // Handle Item List
            if (selectedItem.type == ItemType.equipment || selectedItem.amount <= quantity)
            {
                // Remove the item completely
                Program.player.ownedItems.Remove(selectedItem);
            }
            else
            {
                // Just subtract the amount
                selectedItem.amount -= quantity;
            }

            // Re-apply artifact stats if some items are left
            if (selectedItem.type == ItemType.Artifact && Program.player.ownedItems.Contains(selectedItem))
            {
                inventory.ApplyEffects(selectedItem, null); 
            }


            MainUI.WriteInMainArea($"\nDeposited {quantity}x {itemToBank.name}.");
            Thread.Sleep(1000);
            // Loop continues to show the deposit inventory again
        }
    }

    private void WithdrawItems()
    {
        Inventory inventory = new Inventory(Program.player);

        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Select an item to withdraw:");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("nr     Name                      Qty   Location");
            MainUI.WriteInMainArea("--------------------------------------------------");

            if (Program.player.bankItems.Count == 0)
            {
                MainUI.WriteInMainArea("Your bank is empty.");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0. Back");
            }
            else
            {
                for (int i = 0; i < Program.player.bankItems.Count; i++)
                {
                    var (location, item) = Program.player.bankItems[i];
                    MainUI.WriteInMainArea($"{i + 1,-7}{item.name,-25} {item.amount,-5} {location}");
                }
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("0. Back");
            }

            string inputString = Console.ReadLine() ?? "";
            var n = int.TryParse(inputString, out int input);

            if (input == 0)
            {
                return; 
            }

            if (!n || input < 1 || input > Program.player.bankItems.Count)
            {
                MainUI.WriteInMainArea("\nInvalid selection. Please type a number from the list.");
                Thread.Sleep(1000);
                continue;
            }

            var (loc, selectedItem) = Program.player.bankItems[input - 1];
            int quantity = 1;

            // If stackable, ask how many
            if (selectedItem.type != ItemType.equipment)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"How many {selectedItem.name} would you like to withdraw? (Max: {selectedItem.amount})");
                string quantityString = Console.ReadLine() ?? "";
                var q = int.TryParse(quantityString, out quantity);

                if (!q || quantity < 1 || quantity > selectedItem.amount)
                {
                    MainUI.WriteInMainArea("\nInvalid amount.");
                    Thread.Sleep(1000);
                    continue;
                }
            }


            inventory.AddItem(selectedItem, quantity);

            // --- Remove from bank ---
            if (selectedItem.type == ItemType.equipment || selectedItem.amount == quantity)
            {
                // Remove the item completely from bank
                Program.player.bankItems.RemoveAt(input - 1);
            }
            else
            {
                // Just subtract the amount
                selectedItem.amount -= quantity;
                
            }

            MainUI.WriteInMainArea($"\nWithdrew {quantity}x {selectedItem.name}.");
            Thread.Sleep(1000);
        }
    }


    private void DepositMoney()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Deposit Money");
        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea($"Bank Account: {Program.player.bankMoney}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("How much would you like to deposit? (Type 0 to cancel)");

        string inputString = Console.ReadLine() ?? "";
        var n = int.TryParse(inputString, out int amount);

        if (!n || amount < 0)
        {
            MainUI.WriteInMainArea("\nInvalid amount.");
            Thread.Sleep(1000);
            return;
        }

        if (amount == 0)
        {
            return;
        }

        if (amount > Program.player.money)
        {
            MainUI.WriteInMainArea("\nYou don't have that much Rai.");
            Thread.Sleep(1000);
            return;
        }

        Program.player.money -= amount;
        Program.player.bankMoney += amount;

        MainUI.WriteInMainArea($"\nDeposited {amount} Rai.");
        MainUI.WriteInMainArea($"New balance: {Program.player.bankMoney} Rai.");
        Thread.Sleep(1500);
    }

    private void WithdrawMoney()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Withdraw Money");
        MainUI.WriteInMainArea($"Current Rai: {Program.player.money}");
        MainUI.WriteInMainArea($"Bank Account: {Program.player.bankMoney}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("How much would you like to withdraw? (Type 0 to cancel)");

        string inputString = Console.ReadLine() ?? "";
        var n = int.TryParse(inputString, out int amount);

        if (!n || amount < 0)
        {
            MainUI.WriteInMainArea("\nInvalid amount.");
            Thread.Sleep(1000);
            return;
        }

        if (amount == 0)
        {
            return;
        }

        if (amount > Program.player.bankMoney)
        {
            MainUI.WriteInMainArea("\nYou don't have that much Rai in your account.");
            Thread.Sleep(1000);
            return;
        }

        Program.player.bankMoney -= amount;
        Program.player.money += amount;

        MainUI.WriteInMainArea($"\nWithdrew {amount} Rai.");
        MainUI.WriteInMainArea($"New balance: {Program.player.bankMoney} Rai.");
        Thread.Sleep(1500);
    }
    #endregion

    #region casino
    void BlackjackLogic()
    {
        MainUI.WriteInMainArea($"how much do you want to bet?  current Rai: {Program.player.money} \nthe max bet is {casinoMaxBet}");
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
            MainUI.WriteInMainArea("\nyou dont have that much Rai\n ");
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

            MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
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

            MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
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

            MainUI.WriteInMainArea( "\nGamble some more!! : 1\nLeave : 0");
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

            MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
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

            MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
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
        MainUI.WriteInMainArea($"\nhow much do you want to bet?  current Rai: {Program.player.money} \nthe max bet is {casinoMaxBet}");
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
            MainUI.WriteInMainArea("\nyou dont have that much Rai\n ");
            DoSubLocation();
            return;
        }
        MainUI.WriteInMainArea($"you bet: {bet}\n");
        Program.player.money -= bet;
        Program.SavePlayer();

        MainUI.WriteInMainArea($"where would you like to bet, \nblack : 1 \nred : 2 \neven : 3 \nodd : 4 \n1st 12 : 5 " +
            $"\n2nd 12 : 6 \n3rd 12 : 7 \n1 to 18(half) : 8 \n19 to 36(half) : 9 \nspecific number : 0\n");


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
            case 0:
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

        MainUI.WriteInMainArea($"you have {Program.player.money} Rai");

        MainUI.WriteInMainArea("\nGamble some more!! : 1\nLeave : 0");
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

        MainUI.WriteInMainArea("You ready to start fishing?\nStart fishing : Enter \nGo back to menu : 0");
        string input = Console.ReadKey().KeyChar.ToString();
        int.TryParse(input, out int r);
        if (r==0)
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

                        MainUI.WriteInMainArea("Press Enter to continue... ");
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

                MainUI.WriteInMainArea("Press Enter to continue...");
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

    #region graveyard
    void GraveyardLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Welcome to the Graveyard...");
        MainUI.WriteInMainArea("Here lie the spirits of fallen warriors who died in this town.");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Loading spirits...");
        Thread.Sleep(500);
        
        List<string> allDeadPlayerNames = Program.db.GetAllDeadPlayerNames();
        List<string> deadPlayerNames = new List<string>();
        
        foreach (var name in allDeadPlayerNames)
        {
            var deadPlayer = Program.db.LoadDeadPlayer(name);
            if (deadPlayer != null)
            {
                if (deadPlayer.currentLocation == Program.player.currentLocation)
                {
                    deadPlayerNames.Add(name);
                }
            }
        }
        
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Welcome to the Graveyard...");
        MainUI.WriteInMainArea("Here lie the spirits of fallen warriors who died in this town.");
        MainUI.WriteInMainArea("");
        
        if (deadPlayerNames.Count == 0)
        {
            MainUI.WriteInMainArea("The graveyard is empty. No souls linger here...");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Press Enter to leave");
            Console.ReadLine();
            Program.MainMenu();
            return;
        }

        MainUI.WriteInMainArea($"You sense {deadPlayerNames.Count} restless spirit(s) here.");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Would you like to:");
        MainUI.WriteInMainArea("1. Challenge a spirit to combat");
        MainUI.WriteInMainArea("0. Leave");
        MainUI.WriteInMainArea("");

        int.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice);
        
        if (choice == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }
        else if (choice == 1)
        {
            ShowDeadPlayersList(deadPlayerNames);
        }
        else
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Invalid choice.");
            Thread.Sleep(1000);
            DoSubLocation();
        }
    }

    void ShowDeadPlayersList(List<string> deadPlayerNames)
    {
        int currentPage = 1;
        int itemsPerPage = 8;
        string searchTerm = "";
        
        while (true)
        {
            List<Player> deadPlayers = new List<Player>();
            foreach (var name in deadPlayerNames)
            {
                var p = Program.db.LoadDeadPlayer(name);
                if (p != null)
                {
                    deadPlayers.Add(p);
                }
            }
            
            List<Player> filteredPlayers;
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredPlayers = deadPlayers;
            }
            else
            {
                filteredPlayers = deadPlayers
                    .Where(p => p.name.ToLower().Contains(searchTerm.ToLower()) ||
                               p.playerClass.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }
            
            int totalItems = filteredPlayers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            List<Player> pagePlayers = filteredPlayers
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("=== Restless Spirits ===");
            MainUI.WriteInMainArea("");
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                MainUI.WriteInMainArea($"Searching for: \"{searchTerm}\"");
                MainUI.WriteInMainArea("");
            }

            MainUI.WriteInMainArea("Nr     Name                 Level  Class            HP");
            MainUI.WriteInMainArea("----------------------------------------------------------------");
            
            for (int i = 0; i < pagePlayers.Count; i++)
            {
                var p = pagePlayers[i];
                MainUI.WriteInMainArea($"{i + 1,-7}{p.name,-20} {p.level,-7}{p.playerClass,-17}{p.HP}/{p.maxHP}");
            }
            
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {currentPage} of {totalPages} ---");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Type spirit number (1-8) to challenge, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back");

            string input = Console.ReadKey().KeyChar.ToString().ToLower();

            if (input == "n")
            {
                if (currentPage < totalPages) currentPage++;
                continue;
            }
            if (input == "p")
            {
                if (currentPage > 1) currentPage--;
                continue;
            }
            if (input == "s")
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
                MainUI.WriteInMainArea("> ");
                searchTerm = Console.ReadLine()?.ToLower() ?? "";
                currentPage = 1;
                continue;
            }

            if (int.TryParse(input, out int selection))
            {
                if (selection == 0)
                {
                    DoSubLocation();
                    return;
                }
                
                if (selection >= 1 && selection <= pagePlayers.Count)
                {
                    Player selectedPlayer = pagePlayers[selection - 1];
                    ChallengeDeadPlayer(selectedPlayer.name);
                    return;
                }
            }

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Invalid choice. Please try again.");
            Thread.Sleep(1000);
        }
    }

    void ChallengeDeadPlayer(string deadPlayerName)
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"Loading spirit of {deadPlayerName}...");
        
        Player deadPlayer = Program.db.LoadDeadPlayer(deadPlayerName);
        
        if (deadPlayer == null)
        {
            MainUI.WriteInMainArea($"The spirit of {deadPlayerName} has moved on...");
            MainUI.WriteInMainArea("They are no longer here.");
            Thread.Sleep(2000);
            DoSubLocation();
            return;
        }

        MainUI.WriteInMainArea($"You have summoned the spirit of {deadPlayer.name}!");
        MainUI.WriteInMainArea($"Level {deadPlayer.level} {deadPlayer.playerClass}");
        MainUI.WriteInMainArea($"HP: {deadPlayer.HP}/{deadPlayer.maxHP}");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Prepare for battle!");
        Thread.Sleep(2000);

        Enemy spiritEnemy = ConvertPlayerToEnemy(deadPlayer);
        
        Program.pendingDeadPlayerUpdate = deadPlayer;
        Program.pendingSpiritEnemy = spiritEnemy;
        
        List<Enemy> enemies = new List<Enemy> { spiritEnemy };
        CombatManager combat = new CombatManager(Program.player, enemies, false, null);
        combat.StartCombat();

        Program.pendingDeadPlayerUpdate = null;
        Program.pendingSpiritEnemy = null;

        if (Program.player.IsAlive())
        {
            CombatUI ui = new CombatUI();
            ui.AddToLog("");
            ui.AddToLog($"You have defeated the spirit of {deadPlayer.name}!");
            ui.AddToLog("Their soul has been laid to rest...");
            ui.RenderCombatScreen(Program.player, new List<Combatant> { Program.player });
            
            Program.db.DeleteDeadPlayer(deadPlayer.name);
            
            Thread.Sleep(2000);
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
            
            Program.MainMenu();
        }
    }

    Enemy ConvertPlayerToEnemy(Player player)
    {
        int moneyDrop = (int)Math.Round(player.money * 0.1);
        
        Enemy enemy = new Enemy
        {
            name = player.name + " (Spirit)",
            level = player.level,
            HP = player.HP,
            maxHP = player.maxHP,
            speed = player.speed,
            armor = player.armor,
            dodge = player.dodge,
            dodgeNegation = player.dodgeNegation,
            critChance = player.critChance,
            critDamage = player.critDamage,
            stun = player.stun,
            stunNegation = player.stunNegation,
            money = moneyDrop,
            exp = player.level * 10,
            attacks = new List<Attack>()
        };

        foreach (var attack in player.equippedAttacks)
        {
            if (attack != null)
            {
                enemy.attacks.Add(attack);
            }
        }

        if (enemy.attacks.Count == 0)
        {
            enemy.attacks.Add(new Attack("Spirit Strike", new List<AttackEffect>
            {
                new AttackEffect("damage", 5, 0, "enemy")
            }));
        }

        return enemy;
    }
    #endregion

    #region port
    void PortLogic()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("=== Port Travel ===");
        MainUI.WriteInMainArea("You can travel to other ports you've discovered.");
        MainUI.WriteInMainArea("");

        // Get all port locations
        List<Location> portLocations = new List<Location>();
        foreach (var loc in LocationLibrary.locations)
        {
            bool hasPort = false;
            foreach (var subLoc in loc.subLocationsHere)
            {
                if (subLoc.type == SubLocationType.port)
                {
                    hasPort = true;
                    break;
                }
            }
            if (hasPort && loc.name != Program.player.currentLocation)
            {
                portLocations.Add(loc);
            }
        }

        if (portLocations.Count == 0)
        {
            MainUI.WriteInMainArea("No other ports are available for travel.");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Press Enter to leave");
            Console.ReadLine();
            Program.MainMenu();
            return;
        }

        MainUI.WriteInMainArea("Available Ports:");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Nr     Port Name            Status");
        MainUI.WriteInMainArea("----------------------------------------");

        int displayIndex = 1;
        List<Location> availablePorts = new List<Location>();
        
        foreach (var port in portLocations)
        {
            if (Program.player.knownLocationnames.Contains(port.name))
            {
                MainUI.WriteInMainArea($"{displayIndex,-7}{port.name,-20} Available");
                availablePorts.Add(port);
                displayIndex++;
            }
            else
            {
                MainUI.WriteInMainArea($"?      ???                  Undiscovered");
            }
        }

        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("0. Leave");
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Select a port to travel to:");

        string input = Console.ReadLine() ?? "";
        if (!int.TryParse(input, out int choice))
        {
            MainUI.WriteInMainArea("\nInvalid input.");
            Thread.Sleep(1000);
            DoSubLocation();
            return;
        }

        if (choice == 0)
        {
            MainUI.ClearMainArea();
            Program.MainMenu();
            return;
        }

        if (choice < 1 || choice > availablePorts.Count)
        {
            MainUI.WriteInMainArea("\nInvalid selection.");
            Thread.Sleep(1000);
            DoSubLocation();
            return;
        }

        Location selectedPort = availablePorts[choice - 1];
        
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea($"Traveling to {selectedPort.name}...");
        Thread.Sleep(1500);
        
        Program.player.currentLocation = selectedPort.name;
        Program.SavePlayer();
        
        MainUI.WriteInMainArea($"You have arrived at {selectedPort.name}!");
        Thread.Sleep(1000);
        
        Program.MainMenu();
    }
    #endregion


    //not done



}
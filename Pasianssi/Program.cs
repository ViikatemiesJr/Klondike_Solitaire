using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Pasianssi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool Quit = false;
            winSize();
            while (!Quit)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                bool restart = false; int deckFlips = 0;
                string[] Stacks = new string[4]; // 0 Clubs, 1 Hearts, 2 Spades, 3 Diamonds
                for (int i = 0; i < 4; i++) Stacks[i] = "--";
                List<string> deck = createDeck();
                List<string> dump = new List<string>();
                List<string> line1 = createLine(1, deck);
                List<string> line2 = createLine(2, deck);
                List<string> line3 = createLine(3, deck);
                List<string> line4 = createLine(4, deck);
                List<string> line5 = createLine(5, deck);
                List<string> line6 = createLine(6, deck);
                List<string> line7 = createLine(7, deck);
                string[,] hiddenCards = createHiddenCards(deck); // x,y ## L->R, U->D
                bool limitFlips = limitFlipsQ();
                bool drawOnlyOne = drawOnlyOneQ();
                while (!restart)
                {
                    int[] lineCount = { line1.Count, line2.Count, line3.Count, line4.Count, line5.Count, line6.Count, line7.Count};
                    printHud(deck, dump, line1, line2, line3, line4, line5, line6, line7, Stacks, lineCount, deckFlips);
                    if (deck.Count == 0 && dump.Count == 0)
                    {
                        restart = checkForWin(line1, line2, line3, line4, line5, line6, line7);
                    }
                    bool validMove = false; string[] selection = new string[3];
                    if (!restart) selection = selectionStructure(lineCount); // 1st colum or T or D // 1st row // 2nd colum or chsd }                                       
                    switch (selection[0])
                    {
                        case "Q":
                            Quit = true; restart = true;
                            break;
                        case "R":
                            restart = true; 
                            break;
                        case "D":
                            deckFlips = drawCards(deck, deckFlips, dump, limitFlips, drawOnlyOne);
                            break;
                        case "F":
                            break;
                        default:
                            bool rd3value = validFirstcard(selection, dump, line1, line2, line3, line4, line5, line6, line7);
                            if (rd3value)
                            {
                                switch (selection[2]) 
                                {
                                    case "C":
                                        validMove = moveCardToStack(selection, Stacks, dump, line1, line2, line3, line4, line5, line6, line7);
                                        break;
                                    default:
                                        validMove = moveCardToLine(selection, Stacks, dump, lineCount, line1, line2, line3, line4, line5, line6, line7);
                                        break;
                                }
                                if (validMove) revealHiddenCards(selection, hiddenCards, line1, line2, line3, line4, line5, line6, line7);
                            }
                            break;
                    }
                    
                    Console.Clear();
                }
            }
        }
        static bool checkForWin(List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            //line1[0] != "XX" && 
            bool win = false; int emptyLineCount = 0;
            if (line1.Count == 0) emptyLineCount += 1;
            else if (line1[0] == "XX") return win;
            if (line2.Count == 0) emptyLineCount += 1;
            else if (line2[0] == "XX") return win;
            if (line3.Count == 0) emptyLineCount += 1;
            else if (line3[0] == "XX") return win;
            if (line4.Count == 0) emptyLineCount += 1;
            else if (line4[0] == "XX") return win;
            if (line5.Count == 0) emptyLineCount += 1;
            else if (line5[0] == "XX") return win;
            if (line6.Count == 0) emptyLineCount += 1;
            else if (line6[0] == "XX") return win;
            if (line7.Count == 0) emptyLineCount += 1;
            else if (line7[0] == "XX") return win;
            if (emptyLineCount >= 3)
            {
                win = true;
                Console.WriteLine("\n  YOU WIN!!!\n  Press [Enter] to start a new game.");
                Console.ReadLine();
            }
            return win;
        }
        static void winSize()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            try
            {
                try { Console.SetWindowSize(75, 50); }
                catch { Console.SetWindowSize(70, 40); Console.WriteLine("\nCould not automatically resize console window to optimal size.\n\nPress [Enter] to start the game."); Console.ReadLine(); Console.Clear(); }
            }
            catch
            {
                Console.WriteLine("\nCould not automatically resize console window to optimal size.\nYou might need to resize it manually at some point.\n\nPress [Enter] to start the game.");
                Console.ReadLine(); Console.Clear();
            }
#pragma warning restore CA1416 // Validate platform compatibility
        }
        static bool moveCardToLine(string[] selection, string[] Stacks, List<string> dump, int[] lineCount, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            string[,] fromAndToCards = fetchCards(selection, Stacks, dump, line1, line2, line3, line4, line5, line6, line7);
            bool validMove = false; bool validColor = false;
            if (fromAndToCards[1, 1] == "C" || fromAndToCards[1, 1] == "S")
            {
                if (fromAndToCards[1, 0] == "H" || fromAndToCards[1, 0] == "D") validColor = true;
                else { invalidMove(); return validMove; }
            }
            else if (fromAndToCards[1, 1] == "H" || fromAndToCards[1, 1] == "D")
            {
                if (fromAndToCards[1, 0] == "C" || fromAndToCards[1, 0] == "S") validColor = true;
                else { invalidMove(); return validMove; }
            }
            else
            {
                validColor = true;
            }
            if (Convert.ToInt32(fromAndToCards[0, 0]) == Convert.ToInt32(fromAndToCards[0, 1]) - 1 && validColor) 
            {
                if (selection[1] == "N")
                {
                    switch (selection[2])
                    {
                        case "1":
                            line1.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "2":
                            line2.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "3":
                            line3.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "4":
                            line4.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "5":
                            line5.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "6":
                            line6.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                        case "7":
                            line7.Add(dump[dump.Count - 1]);
                            dump.RemoveAt(dump.Count - 1);
                            break;
                    }                    
                }
                else
                {
                    switch (selection[2])
                    {
                        case "1":
                            moveCardToLineSub(line1, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "2":
                            moveCardToLineSub(line2, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "3":
                            moveCardToLineSub(line3, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "4":
                            moveCardToLineSub(line4, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "5":
                            moveCardToLineSub(line5, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "6":
                            moveCardToLineSub(line6, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                        case "7":
                            moveCardToLineSub(line7, selection, lineCount, line1, line2, line3, line4, line5, line6, line7);
                            break;
                    }
                }
                validMove = true;
            }
            else invalidMove();
            return validMove;
        }
        static void moveCardToLineSub(List<string> targetLine, string[] selection, int[] lineCount, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            int row = Convert.ToInt32(selection[1]) - 1;
            switch (selection[0])
            {
                case "1":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line1[row]);
                        line1.RemoveAt(row);
                    }
                    break;
                case "2":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line2[row]);
                        line2.RemoveAt(row);
                    }
                    break;
                case "3":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line3[row]);
                        line3.RemoveAt(row);
                    }
                    break;
                case "4":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line4[row]);
                        line4.RemoveAt(row);
                    }
                    break;
                case "5":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line5[row]);
                        line5.RemoveAt(row);
                    }
                    break;
                case "6":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line6[row]);
                        line6.RemoveAt(row);
                    }
                    break;
                case "7":
                    for (int i = row; i < lineCount[Convert.ToInt32(selection[0]) - 1]; i++)
                    {
                        targetLine.Add(line7[row]);
                        line7.RemoveAt(row);
                    }
                    break;
            }
        }
        static void revealHiddenCards(string[] selection, string[,] hiddenCards, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            int nro;
            if (selection[1] == "N") nro = -1;
            else nro = Convert.ToInt32(selection[1]);
            if (nro > 1)
            {
                switch (selection[0])
                {
                    case "2":
                        if (line2[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line2.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line2.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                    case "3":
                        if (line3[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line3.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line3.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                    case "4":
                        if (line4[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line4.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line4.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                    case "5":
                        if (line5[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line5.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line5.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                    case "6":
                        if (line6[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line6.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line6.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                    case "7":
                        if (line7[Convert.ToInt32(selection[1]) - 2] == "XX")
                        {
                            line7.RemoveAt(Convert.ToInt32(selection[1]) - 2);
                            line7.Add(hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2]);
                            hiddenCards[Convert.ToInt32(selection[0]) - 1, Convert.ToInt32(selection[1]) - 2] = "";
                        }
                        break;
                }
            }
        }
        static bool moveCardToStack(string[] selection, string[] Stacks, List<string> dump, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            string[,] fromAndToCards = fetchCards(selection, Stacks, dump, line1, line2, line3, line4, line5, line6, line7);
            bool validMove = false;
            if (Convert.ToInt32(fromAndToCards[0, 0]) == Convert.ToInt32(fromAndToCards[0, 1]) + 1)
            {            
                switch (fromAndToCards[1, 0])
                {
                    case "C":                    
                        Stacks[0] = fromAndToCards[1, 0] + fromAndToCards[0, 0];                       
                        break;
                    case "H":
                        Stacks[1] = fromAndToCards[1, 0] + fromAndToCards[0, 0];
                        break;
                    case "S":
                        Stacks[2] = fromAndToCards[1, 0] + fromAndToCards[0, 0];
                        break;
                    case "D":
                        Stacks[3] = fromAndToCards[1, 0] + fromAndToCards[0, 0];
                        break;
                }
                switch (selection[0])
                {
                    case "1":
                        line1.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "2":
                        line2.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "3":
                        line3.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "4":
                        line4.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "5":
                        line5.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "6":
                        line6.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "7":
                        line7.RemoveAt(Convert.ToInt32(selection[1]) - 1);
                        break;
                    case "T":
                        dump.RemoveAt(dump.Count - 1);
                        break;
                }
                validMove = true;
            }
            else invalidMove();
            return validMove;
        }
        static void invalidMove()
        {
            Console.WriteLine("Cannot do this card move, press [Enter] to continue game.");
            Console.ReadLine();
        }
        static string[,] fetchCards(string[] selection, string[] Stacks, List<string> dump, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            string[,] fromAndToCards = new string[2,2]; string cardTemp = ""; string[] cardTemp2; bool emptyStack = false; bool emptyLine = false;
            if (selection[1] == "N")
            {
                cardTemp = dump[dump.Count - 1];
            }
            else
            {
                switch (selection[0])
                {
                    case "1":
                        cardTemp = line1[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "2":
                        cardTemp = line2[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "3":
                        cardTemp = line3[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "4":
                        cardTemp = line4[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "5":
                        cardTemp = line5[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "6":
                        cardTemp = line6[Convert.ToInt32(selection[1]) - 1];
                        break;
                    case "7":
                        cardTemp = line7[Convert.ToInt32(selection[1]) - 1];
                        break;
                }
            }
            cardTemp2 = suitSeparator(cardTemp);
            fromAndToCards[0, 0] = cardTemp2[0];
            fromAndToCards[1, 0] = cardTemp2[1];
            switch (selection[2])
            {
                case "1":
                    if (line1.Count == 0) emptyLine = true;
                    else cardTemp = line1[line1.Count - 1];
                    break;
                case "2":
                    if (line2.Count == 0) emptyLine = true;
                    else cardTemp = line2[line2.Count - 1];
                    break;
                case "3":
                    if (line3.Count == 0) emptyLine = true;
                    else cardTemp = line3[line3.Count - 1];
                    break;
                case "4":
                    if (line4.Count == 0) emptyLine = true;
                    else cardTemp = line4[line4.Count - 1];
                    break;
                case "5":
                    if (line5.Count == 0) emptyLine = true;
                    else cardTemp = line5[line5.Count - 1];
                    break;
                case "6":
                    if (line6.Count == 0) emptyLine = true;
                    else cardTemp = line6[line6.Count - 1];
                    break;
                case "7":
                    if (line7.Count == 0) emptyLine = true;
                    else cardTemp = line7[line7.Count - 1];
                    break;
                case "C":
                    switch (fromAndToCards[1, 0])
                    {
                        case "C":
                            if (Stacks[0] == "--") emptyStack = true;
                            else cardTemp = Stacks[0];
                            break;
                        case "H":
                            if (Stacks[1] == "--") emptyStack = true;
                            else cardTemp = Stacks[1];
                            break;
                        case "S":
                            if (Stacks[2] == "--") emptyStack = true;
                            else cardTemp = Stacks[2];
                            break;
                        case "D":
                            if (Stacks[3] == "--") emptyStack = true;
                            else cardTemp = Stacks[3];
                            break;
                    }
                    break;
            }
            if (!emptyStack && !emptyLine)
            {
                cardTemp2 = suitSeparator(cardTemp);
                fromAndToCards[0, 1] = cardTemp2[0];
                fromAndToCards[1, 1] = cardTemp2[1];
            }
            else if (emptyStack)
            {
                fromAndToCards[0, 1] = "0";
                fromAndToCards[1, 1] = fromAndToCards[1, 0];
            }
            else if (emptyLine)
            {
                fromAndToCards[0, 1] = "14";
                fromAndToCards[1, 1] = "-";
            }
            return fromAndToCards;
        }
        static string[] suitSeparator(string card)
        {
            string[] separatedCard = new string[2];
            separatedCard[1] = Convert.ToString(card[0]);
            separatedCard[0] = Convert.ToString(card[1]);
            if (card.Length == 3) separatedCard[0] += Convert.ToString(card[2]);
            return separatedCard;
        }
        static bool validFirstcard(string[] selection, List<string> dump, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7)
        {
            bool rd3value = false;
            if (selection[1] == "N" && dump.Count > 0) return true;
            switch (selection[0])
            {
                case "1":
                    rd3value = validFirstCardSub(line1, selection);
                    break;
                case "2":
                    rd3value = validFirstCardSub(line2, selection);
                    break;
                case "3":
                    rd3value = validFirstCardSub(line3, selection);
                    break;
                case "4":
                    rd3value = validFirstCardSub(line4, selection);
                    break;
                case "5":
                    rd3value = validFirstCardSub(line5, selection);
                    break;
                case "6":
                    rd3value = validFirstCardSub(line6, selection);
                    break;
                case "7":
                    rd3value = validFirstCardSub(line7, selection);
                    break;
            }
            return rd3value;
        }
        static bool validFirstCardSub(List<string> line, string[] selection)
        {
            bool rd3value = false;
            if (line[int.Parse(selection[1]) - 1] == "--" || line[int.Parse(selection[1]) - 1] == null)
            { Console.WriteLine("Invalid first cardlocation, press [ENTER] to continue."); Console.ReadLine(); }
            else rd3value = true;
            return rd3value;
        }
        static int drawCards(List<string> deck, int deckFlips, List<string> dump, bool limitFlips, bool drawOnlyOne)
        {
            Random rnd = new Random(); int rng;
            int tempDeckflips = 0;            
            if (deck.Count > 0 && deckFlips == 0)
            {                
                for (int n = 0; n < 3; n++)
                {
                    if (drawOnlyOne) n += 2;
                    if (deck.Count > 0) { rng = rnd.Next(deck.Count); dump.Add(deck[rng]); deck.RemoveAt(rng); }
                }
            }            
            else
            {
                if (!limitFlips)
                {
                    if (deckFlips > 0 && deck.Count <= 0)
                    {
                        tempDeckflips = deckFlips;
                        deckFlips = 0;
                    }
                }
                if (deck.Count <= 0 && deckFlips < 3)
                {
                    int dC = dump.Count;
                    for (int i = 0; i < dC; i++) { deck.Add(dump[0]); dump.RemoveAt(0); }
                    deckFlips += 1;
                }
                else if (deck.Count <= 0 && deckFlips >= 3) { Console.WriteLine("You have reached maximum deck flips and have run out of your last deck. Press [Enter] to continue game."); Console.ReadLine(); }
                if (deck.Count > 0)
                {
                    for (int n = 0; n < 3; n++)
                    {
                        if (drawOnlyOne) n += 2;
                        if (deck.Count > 0) { dump.Add(deck[0]); deck.RemoveAt(0); }
                    }
                }
                if (!limitFlips) deckFlips += tempDeckflips;
            }            
            return deckFlips;
        }
        static void printHud(List<string> deck, List<string> dump, List<string> line1, List<string> line2, List<string> line3, List<string> line4, List<string> line5, List<string> line6, List<string> line7, string[] stacks, int[] lineCount, int deckFlips)
        {
            string rd, nd, st; rd = nd = st = "--";
            if (dump.Count > 0)
            {
                st = dump[dump.Count -1];
                if (dump.Count > 1)
                {
                    nd = dump[dump.Count -2];
                    if (dump.Count > 2)rd = dump[dump.Count - 3];
                }
            }            
            Console.WriteLine("Klondike Solitaire\t\t\t     By: Viikatemies Jr");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n{0,-5}{1,-15}{2,-5}{3,-5}{4,-5}{5,-5}{0,-5}{6,-10}{7,-15}", null, "[D] Deck #", "[C]", "[H]", "[S]", "[D]", "[Q] Quit", "[R] Restart");
            
            //Console.WriteLine("{0,-5}{1,-15}{2,-5}{3,-5}{4,-5}{5,-5}\n", null, deck.Count, stacks[0], stacks[1], stacks[2], stacks[3]);  
            Console.Write("{0,-5}", null);
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (deck.Count == 0) Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("{0,-15}", deck.Count);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0,-5}", stacks[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("{0,-5}", stacks[1]);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0,-5}", stacks[2]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0,-5}", stacks[3]);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0,-5}{1,-10}{2,-5}{3,-5}{4,-10}{5}", null, "Dump #", "3rd", "2nd", "[T] 1st", "Flips");
            
            //Console.WriteLine("{0,-5}{1,-10}{2,-5}{3,-5}{4,-10}{5}\n", null, dump.Count, rd, nd, st, deckFlips);
            Console.Write("{0,-5}", null);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("{0,-10}", dump.Count);

            if (rd == "--") Console.ForegroundColor = ConsoleColor.Yellow;
            else if (rd[0] == 'H' || rd[0] == 'D') Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0,-5}", rd);

            if (nd == "--") Console.ForegroundColor = ConsoleColor.Yellow;
            else if (nd[0] == 'H' || nd[0] == 'D') Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0,-5}", nd);

            if (st == "--") Console.ForegroundColor = ConsoleColor.Yellow;
            else if (st[0] == 'H' || st[0] == 'D') Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0,-10}", st);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(deckFlips + "\n");

            int i = 0;
            for (int j = 0; j < lineCount.Length; j++)
            {
                if (lineCount[j] > i) i = lineCount[j];
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0,-5}{1,-5}{2,-5}{3,-5}{4,-5}{5,-5}{6,-5}{7,-5}\n", null, "[1]", "[2]", "[3]", "[4]", "[5]", "[6]", "[7]");
            for (int j = 0; j < i; j++)
            {
                string l1 = printHudLineSub(j, line1);
                string l2 = printHudLineSub(j, line2);
                string l3 = printHudLineSub(j, line3);
                string l4 = printHudLineSub(j, line4);
                string l5 = printHudLineSub(j, line5);
                string l6 = printHudLineSub(j, line6);
                string l7 = printHudLineSub(j, line7);
                
                //Console.WriteLine("{0,-5}{1,-5}{2,-5}{3,-5}{4,-5}{5,-5}{6,-5}{7,-5}{8,-5}", null, l1, l2, l3, l4, l5, l6, l7, "[" + (j + 1) + "]");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0,-5}", null);

                if (l1 != null && (l1[0] == 'H' || l1[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l1 != null && (l1 == "XX" || l1 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l1); Console.ForegroundColor = ConsoleColor.Gray;

                if (l2 != null && (l2[0] == 'H' || l2[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l2 != null && (l2 == "XX" || l2 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l2); Console.ForegroundColor = ConsoleColor.Gray;

                if (l3 != null && (l3[0] == 'H' || l3[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l3 != null && (l3 == "XX" || l3 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l3); Console.ForegroundColor = ConsoleColor.Gray;

                if (l4 != null && (l4[0] == 'H' || l4[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l4 != null && (l4 == "XX" || l4 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l4); Console.ForegroundColor = ConsoleColor.Gray;

                if (l5 != null && (l5[0] == 'H' || l5[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l5 != null && (l5 == "XX" || l5 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l5); Console.ForegroundColor = ConsoleColor.Gray;

                if (l6 != null && (l6[0] == 'H' || l6[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l6 != null && (l6 == "XX" || l6 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l6); Console.ForegroundColor = ConsoleColor.Gray;

                if (l7 != null && (l7[0] == 'H' || l7[0] == 'D')) Console.ForegroundColor = ConsoleColor.Red;
                else if (l7 != null && (l7 == "XX" || l7 == "--")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0,-5}", l7); Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("{0,-5}", "[" + (j + 1) + "]");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
        }
        static string printHudLineSub(int j, List<string> line)
        {
            int len = line.Count;
            if (len == 0 && j == 0)
                return "--";
            else if (len > j)
                return line[j];
            else
                return null;
        }
        static bool limitFlipsQ()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            bool ret = false;
            Console.WriteLine("Klondike Solitaire\t\t\t     By: Viikatemies Jr");
            Console.WriteLine("\n  Do you want to limit your deck flips to 3? [Y] / [N]");
            if (Console.ReadLine().ToUpper() == "Y") ret = true;
            Console.Clear();
            return ret;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        static bool drawOnlyOneQ()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            bool ret = false;
            Console.WriteLine("Klondike Solitaire\t\t\t     By: Viikatemies Jr");
            Console.WriteLine("\n  Do you want to draw cards 1 at a time instead of reqular 3? [Y] / [N]");
            if (Console.ReadLine().ToUpper() == "Y") ret = true;
            Console.Clear();
            return ret;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        static string[] selectionStructure(int[] lineCount)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string[] selection = new string[3];            
            bool cont = true; bool cont2 = false;
            while (cont)
            {
                Console.WriteLine("\n  First give Column nro [1 to 7] to pick column,\n  [T] to pick top card from dump OR [D] to draw more cards.");
                selection[0] = Console.ReadLine().ToUpper();
                switch (selection[0])
                {
                    case "1": case "2": case "3": case "4": case "5": case "6": case "7":
                        Console.WriteLine("  From which row you want to choose the card?\n  [N] to Cancel move");
                        selection[1] = Console.ReadLine().ToUpper();
                        try
                        {
                            if (int.Parse(selection[1]) > 0 && int.Parse(selection[1]) <= lineCount[int.Parse(selection[0]) - 1]) cont2 = true;
                            else invalidSelection();
                        }
                        catch
                        {
                            if (selection[1] == "Q" || selection[1] == "R") { cont = false; selection[0] = selection[1]; }
                            else if (selection[1] == "N") { selection[0] = "F"; return selection; }
                            else invalidSelection();
                        }
                        break;
                    case "T":
                        selection[1] = "N"; cont2 = true;
                        break;                    
                    case "D": case "Q": case "R":
                        cont = false;
                        break;
                    case "N":
                        selection[0] = "F";
                        return selection;
                    default:
                        invalidSelection();
                        break;
                }
                if (cont2) 
                {
                    Console.WriteLine("  To which column you want to move the card?\n  In some cases you can move card to [C][H][S][D]\n  [N] to Cancel move");
                    selection[2] = Console.ReadLine().ToUpper();
                    switch (selection[2])
                    {
                        case "1": case "2": case "3": case "4": case "5": case "6": case "7":
                            if (selection[2] == selection[0]) Console.WriteLine("Invalid selection, Cannot move card to same column it came from.");
                            else cont = false;
                            break;
                        case "C": case "H": case "S": case "D":
                            if (selection[1] == "N")
                            { selection[2] = "C"; cont = false; }
                            else if (Convert.ToInt32(selection[1]) == lineCount[Convert.ToInt32(selection[0]) - 1])
                            { selection[2] = "C"; cont = false; }
                            else Console.WriteLine("Invalid selection, Cannot move card that's not on top of it's column to End stacks.");
                            break;
                        case "Q": case "R":
                            cont = false; selection[0] = selection[2];
                            break;
                        case "N":
                            selection[0] = "F";
                            return selection;
                        default:
                            invalidSelection();
                            break;
                    }
                }
            }
            return selection;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        static void invalidSelection()
        {
            Console.WriteLine("Invalid selection");
        }
        static List<string> createLine(int id, List<string> deck)
        {
            Random rnd = new Random(); int rng;
            List<string> line = new List<string>();
            if (id == 7) line.Add("XX");
            if (id >= 6) line.Add("XX");
            if (id >= 5) line.Add("XX");
            if (id >= 4) line.Add("XX");
            if (id >= 3) line.Add("XX");
            if (id >= 2) line.Add("XX");
            rng = rnd.Next(deck.Count); 
            line.Add(deck[rng]); 
            deck.RemoveAt(rng);
            return line;
        }
        static string[,] createHiddenCards(List<string> deck)
        {
            Random rnd = new Random(); int rng;
            string[,] hiddenCards = new string[7, 6]; // x,y ## L->R, U->D
            int offsetX = 1;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0 + offsetX; x < 7; x++)
                {
                    rng = rnd.Next(deck.Count); hiddenCards[x, y] = deck[rng]; deck.RemoveAt(rng);
                }
                offsetX++;
            }
            return hiddenCards;
        }
        static List<string> createDeck()
        {
            List<string> deck = new List<string>();
            for (int j = 0; j < 4; j++)
            {
                string suit = "C";
                switch (j)
                {
                    case 1:
                        suit = "H";
                        break;
                    case 2:
                        suit = "S";
                        break;
                    case 3:
                        suit = "D";
                        break;
                }
                for (int i = 1; i <= 13; i++)
                {
                    deck.Add(suit + Convert.ToString(i));
                }
            }
            return deck;
        }
    }
}
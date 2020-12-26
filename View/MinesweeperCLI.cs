using System;
using System.Drawing;

namespace Minesweeper
{
    /// <summary>
    /// Command line implementation of a minesweeper user interface. Takes user inputs, validates them, invokes the appropriate controller commands, and converts the world state into displayable text. 
    /// </summary>
    class MinesweeperCLI
    {
        /// <summary>
        /// The one and only controller
        /// </summary>
        GameController theController = new GameController();

        bool showControlHints = true;

        static void Main(string[] args)
        {
            MinesweeperCLI cli = new MinesweeperCLI();
            cli.NewGame();
            Console.Read(); // keep console open
        }

        public MinesweeperCLI()
        {
            theController.GameOver += EndGame;

        }

        /// <summary>
        /// Handles when the game ends, whether by blowing up a mine or by winning.
        /// </summary>
        /// <param name="gameIsWon"></param>
        private void EndGame(bool gameIsWon)
        {
            Console.Clear();
            string message;
            if (gameIsWon)
            {
                PrintWorld();
                message = "You win! :D Play again (y/n)?";
            }
            else
            {
                // reveal all tiles
                theController.theMinefield.RevealAllTiles();
                PrintWorld();
                message = "You lost :( Play again (y/n)?";
            }

            Console.WriteLine(message);

            string input = Console.ReadLine();
            while(input != "yes" && input != "no" && input != "y" && input != "n")
            {
                input = Console.ReadLine();
            }
            if (input == "yes" || input == "y")
                NewGame();

            // TODO: else, close the console
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Gets the initial game parameters and starts the game event loop.
        /// </summary>
        private void NewGame()
        {
            Console.Clear();

            int width;
            int height;
            int numMines;

            Console.WriteLine("Welcome to Minesweeper! Input world width (4-26, inclusive):");
            string input = Console.ReadLine();
            while (!int.TryParse(input, out width) || width > 26 || width < 4)
            {
                Console.WriteLine("Number was out of range of improperly formatted. Try again:");
                input = Console.ReadLine();
            }
            Console.WriteLine("Input world height (4-26, inclusive):");
            input = Console.ReadLine();
            while (!int.TryParse(input, out height) || height > 26 || height < 4)
            {
                Console.WriteLine("Number was out of range of improperly formatted. Try again:");
                input = Console.ReadLine();
            }

            Console.WriteLine($"Input number of mines (min 1, max of {width * height} for world size {width}x{height}):");
            input = Console.ReadLine();
            while (!int.TryParse(input, out numMines) || numMines > width * height || numMines < 1)
            {
                Console.WriteLine("Number was out of range of improperly formatted. Try again:");
                input = Console.ReadLine();
            }

            theController.NewGame(width, height, numMines);

            while (true) // should still exit when user presses normal console exit key combo
            {
                RenderFrame();
            }
        }

        /// <summary>
        /// Draws the game board once, then reads input and sends to the controller
        /// </summary>
        private void RenderFrame()
        {
            Console.Clear();
            PrintWorld();
            Console.WriteLine(theController.statusMessage);
            if (showControlHints)
                Console.WriteLine("Use arrow keys to select a space, d to dig, and f to flag (press h to show/hide this message)");
            ConsoleKeyInfo inputKeyInfo = Console.ReadKey(false);
            switch (inputKeyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    theController.MoveCursorUp();
                    break;
                case ConsoleKey.DownArrow:
                    theController.MoveCursorDown();
                    break;
                case ConsoleKey.LeftArrow:
                    theController.MoveCursorLeft();
                    break;
                case ConsoleKey.RightArrow:
                    theController.MoveCursorRight();
                    break;
                case ConsoleKey.D:
                    theController.Dig();
                    break;
                case ConsoleKey.H:
                    showControlHints = !showControlHints;
                    break;
                case ConsoleKey.F:
                    theController.ToggleFlag();
                    break;
            }
        }

        /// <summary>
        /// Prints the current world state to the console
        /// </summary>
        private void PrintWorld()
        {
            // print letter guides
            // Should use C# equivalent to Java StringBuilder in next version
            string lineToWrite = "    ";
            for (int i = 0; i < theController.theMinefield.Width; i++)
                lineToWrite += $"{(char)('a' + i)} ";

            Console.WriteLine(lineToWrite);

            lineToWrite = "    _";
            for (int i = 0; i < theController.theMinefield.Width * 2; i++)
                lineToWrite += "_";

            Console.WriteLine(lineToWrite);

            // data is stored in columns of rows, but we have to get it to rows of columns in order to print it out
            for (int row = 0; row < theController.theMinefield.Height; row++)
            {
                Console.Write((row + 1));

                // print white space to align left border regardless of number of digits
                for (int i = 0; i < (3 - (row + 1).ToString().Length); i++)
                    Console.Write(" ");

                Console.Write("|");

                for (int col = 0; col < theController.theMinefield.Width; col++)
                {
                    String toWrite = "";
                    Tile t = theController.theMinefield.GetTile(col, row);
                    if (t.IsFlagged)
                        toWrite += "f ";
                    else if (t.IsHidden)
                        toWrite += "  ";
                    else if (t.IsMine)
                        toWrite += "M ";
                    else if (t.Value == 0)
                        toWrite += "  ";
                    else
                        toWrite += t.Value + " ";

                    if (t.IsHidden)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        if (t.IsMine)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            switch(t.Value)
                            {
                                case 0:
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    break;
                                case 1:
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case 2:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 3:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 4:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    break;
                            }
                        }
                    }

                    // invert if the cursor is over the current cell
                    if (row == theController.theMinefield.SelectedRow && col == theController.theMinefield.SelectedCol)
                    {
                        if (t.IsHidden || (!t.IsHidden && t.Value == 0))
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            // swap colors
                            ConsoleColor oldBgColor = Console.BackgroundColor;
                            Console.BackgroundColor = Console.ForegroundColor;
                            Console.ForegroundColor = oldBgColor;
                        }

                    }

                    Console.Write(toWrite);

                    // reset colors
                    Console.ResetColor();
                }

                // new line
                Console.WriteLine();
            }
        }
    }
}

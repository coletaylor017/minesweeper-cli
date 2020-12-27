using System;
using System.Drawing;
using System.Text;

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

        private bool showControlHints = true;

        /// <summary>
        /// Whether or not the game board renders in color. Changes the mechanics of the board rendering as well
        /// </summary>
        private bool colorsOn;

        private ConsoleColor accentColor = ConsoleColor.Cyan;

        private ConsoleColor errorColor = ConsoleColor.Yellow;

        /// <summary>
        /// Delegate for checking whether an input string is valid or not
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        delegate bool ConsoleInputValidator(string input);

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // allow flag & mine characters

            MinesweeperCLI cli = new MinesweeperCLI();
            cli.NewGame();
            Console.Read(); // keep console open
        }

        public MinesweeperCLI()
        {
            theController.GameOver += EndGame;

            WriteRainbowLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║  |  \/  |(_)                                                             ║
║  | .  . | _  _ __    ___  ___ __      __  ___   ___  _ __    ___  _ __   ║
║  | |\/| || || '_ \  / _ \/ __|\ \ /\ / / / _ \ / _ \| '_ \  / _ \| '__|  ║
║  | |  | || || | | ||  __/\__ \ \ V  V / |  __/|  __/| |_) ||  __/| |     ║
║  \_|  |_/|_||_| |_| \___||___/  \_/\_/   \___| \___|| .__/  \___||_|     ║
║                                                     | |                  ║
║                                                     |_|                  ║
╚══════════════════════════════════════════════════════════════════════════╝
                                                          © Cole Taylor 2020
");
            Console.WriteLine();
            WriteInColor("Welcome to Minesweeper! ", accentColor);
            Console.Write("Would you like the board to render in color? ");
            WriteYesOrNo();
            Console.WriteLine();
            Console.WriteLine("(Note: color games run slower, esp. for large boards)");
            string input = GetValidStringInput(
                s => s == "yes" || s == "no" || s == "y" || s == "n",
                "Input not recognized. Type 'yes', 'no', 'y', or 'n'"
            );
            colorsOn = input == "yes" || input == "y";
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
                message = "You win! :D ";
            }
            else
            {
                // reveal all tiles
                theController.theMinefield.RevealAllTiles();
                PrintWorld();
                message = "You lost :( ";
            }
            Console.Write(message);
            Console.Write("Play again? ");
            WriteYesOrNo();
            Console.WriteLine();

            string input = GetValidStringInput(s => s == "yes" || s == "no" || s == "y" || s == "n",
                "Input not recognized. Type 'yes', 'no', 'y', or 'n'");

            if (input == "yes" || input == "y")
                NewGame();

            Environment.Exit(0);
        }
        
        /// <summary>
        /// Gets the initial game parameters and starts the game event loop.
        /// </summary>
        private void NewGame()
        {
            Console.Clear();

            WriteRainbowLine(@"
╔══════════════════════════════════════════════════════════════════════════╗
║     _____                             _____        _                     ║
║    |  __ \                           /  ___|      | |                    ║
║    | |  \/  __ _  _ __ ___    ___    \ `--.   ___ | |_  _   _  _ __      ║
║    | | __  / _` || '_ ` _ \  / _ \    `--. \ / _ \| __|| | | || '_ \     ║
║    | |_\ \| (_| || | | | | ||  __/   /\__/ /|  __/| |_ | |_| || |_) |    ║
║     \____/ \__,_||_| |_| |_| \___|   \____/  \___| \__| \__,_|| .__/     ║
║                                                               | |        ║
║                                                               |_|        ║
╚══════════════════════════════════════════════════════════════════════════╝
");

            WriteInColor("Input board width", accentColor);
            Console.WriteLine(" (4-26, inclusive):");
            int width = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= 26 && n >= 4), 
                "Number was out of range of improperly formatted. Try again:"
            ));

            WriteInColor("Input board height", accentColor);
            Console.WriteLine(" (4-26, inclusive):");
            int height = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= 26 && n >= 4), 
                "Number was out of range of improperly formatted.Try again:"
            ));

            WriteInColor("Input number of mines", accentColor);
            Console.WriteLine($" (min 1, max of {width * height} for board size {width}x{height}):");
            int numMines = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= width * height && n >= 1), 
                "Number was out of range of improperly formatted.Try again:"
            ));

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

                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < theController.theMinefield.Width; col++)
                {
                    string toWrite = "";
                    Tile t = theController.theMinefield.GetTile(col, row);
                    if (t.IsFlagged)
                        toWrite += "▲ ";
                    else if (t.IsHidden)
                        toWrite += colorsOn ? "  " : "░░";
                    else if (t.IsMine)
                        toWrite += "҉ ";
                    else if (t.Value == 0)
                        toWrite += "  ";
                    else
                        toWrite += t.Value + " ";

                    if (colorsOn)
                    {
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
                                switch (t.Value)
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
                    }

                    // invert if the cursor is over the current cell
                    if (row == theController.theMinefield.SelectedRow && col == theController.theMinefield.SelectedCol)
                    {
                        if (colorsOn)
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
                        else
                        {
                            toWrite = "╬ ";
                        }
                    }

                    // if we are in color mode, write string and reset for the next one
                    if (colorsOn)
                    {
                        Console.Write(toWrite);
                        Console.ResetColor();
                    }
                    // if in black and white mode, add string to the string builder
                    else
                    {
                        sb.Append(toWrite);
                    }
                }

                // end of line
                if (colorsOn)
                    // new line
                    Console.WriteLine();
                else
                    Console.WriteLine(sb.ToString());

            }
        }

        /// <summary>
        /// Waits for the user to input a valid line, prompting the given error message until the input meets the specifications designated in the delegate
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string GetValidStringInput(ConsoleInputValidator isValid, string errorMessage)
        {
            string input = Console.ReadLine();
            while (!isValid(input))
            {
                WriteInColor(errorMessage, errorColor);
                Console.WriteLine();
                input = Console.ReadLine();
            }
            return input;
        }

        /// <summary>
        /// Prints the desired string in rainbow text.
        /// </summary>
        /// <param name="line"></param>
        private void WriteRainbowLine(string line)
        {
            ConsoleColor[] rainbowSequence =
            {
                ConsoleColor.DarkRed,
                ConsoleColor.Red,
                ConsoleColor.DarkYellow,
                ConsoleColor.Cyan,
                ConsoleColor.Green,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.Blue,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkMagenta,
                ConsoleColor.Magenta
            };

            for (int i = 0; i < line.Length; i++)
            {
                Console.ForegroundColor = rainbowSequence[i % rainbowSequence.Length];
                Console.Write(line[i]);
            }
            Console.WriteLine();

            Console.ResetColor();
        }

        /// <summary>
        /// Writes a colorful y/n to the console
        /// </summary>
        private void WriteYesOrNo()
        {
            Console.ResetColor();
            Console.Write("(");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("y");
            Console.ResetColor();
            Console.Write("/");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("n");
            Console.ResetColor();
            Console.Write(")");
        }

        /// <summary>
        /// Writes the specified string in the specified color, then resets the console color to its default.
        /// </summary>
        /// <param name="s"></param>
        private void WriteInColor(string s, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.Write(s);
            Console.ResetColor();
        }
    }
}

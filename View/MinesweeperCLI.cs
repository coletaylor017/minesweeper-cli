using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Minesweeper
{
    /// <summary>
    /// Command line implementation of a minesweeper user interface. Takes user inputs, validates them, invokes the appropriate controller commands, and converts the world state into displayable text. 
    /// Constructing this class starts a new game. 
    /// </summary>
    class MinesweeperCLI
    {
        /// <summary>
        /// The one and only controller
        /// </summary>
        GameController theController = new GameController();

        private bool showControlHints = true;

        private ConsoleColor accentColor = ConsoleColor.Cyan;

        private ConsoleColor errorColor = ConsoleColor.Yellow;

        private String helpMessage = "Use arrow keys or HJKL to select a space, d to dig, and f to flag (press F1 to show/hide this message)";

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

        /// <summary>
        /// Constructs a new MinecweeperCLI and all other classes needed for a new Minesweeper game. Then starts the game. 
        /// </summary>
        public MinesweeperCLI()
        {
            theController.GameOver += HandleGameOver;
            theController.BoardUpdated += HandleBoardUpdate;
            theController.InitBoard += HandleInitBoard;

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
            Console.WriteLine("Press any key to start. ");
            Console.ReadKey();
        }

        /// <summary>
        /// Handles when the game ends, whether by blowing up a mine or by winning.
        /// </summary>
        /// <param name="gameIsWon"></param>
        private void HandleGameOver(bool gameIsWon)
        {
            Console.SetCursorPosition(0, theController.theMinefield.Height + 5);
            Console.Write(gameIsWon ? "You win! :D" : "You lost :(");
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
        /// Gets the initial game parameters and then tells the controller to start the game. 
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
            Console.WriteLine(" (4-80, inclusive):");
            int width = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= 80 && n >= 4), 
                "Number was out of range of improperly formatted. Try again:"
            ));

            WriteInColor("Input board height", accentColor);
            Console.WriteLine(" (4-45, inclusive):");
            int height = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= 45 && n >= 4),
                "Number was out of range of improperly formatted.Try again:"
            ));

            WriteInColor("Input number of mines", accentColor);
            Console.WriteLine($" (min 1, max of {width * height} for board size {width}x{height}):");
            int numMines = int.Parse(GetValidStringInput(
                s => (int.TryParse(s, out int n) && n <= width * height && n >= 1), 
                "Number was out of range of improperly formatted.Try again:"
            ));

            theController.NewGame(width, height, numMines);
        }

        /// <summary>
        /// Reads keyboard input and instructs the controller
        /// </summary>
        private void ReadInput()
        {
            ConsoleKeyInfo inputKeyInfo = Console.ReadKey(true);
            switch (inputKeyInfo.Key)
            {
                case ConsoleKey.K:
                case ConsoleKey.UpArrow:
                    theController.MoveCursorUp(); // Moves the cursor up in the model, now move it up visually
                    break;
                case ConsoleKey.J:
                case ConsoleKey.DownArrow:
                    theController.MoveCursorDown();
                    break;
                case ConsoleKey.H:
                case ConsoleKey.LeftArrow:
                    theController.MoveCursorLeft();
                    break;
                case ConsoleKey.L:
                case ConsoleKey.RightArrow:
                    theController.MoveCursorRight();
                    break;
                case ConsoleKey.D:
                    theController.Dig();
                    break;
                case ConsoleKey.F:
                    theController.ToggleFlag();
                    break;
                case ConsoleKey.F1: // toggle control hints
                    showControlHints = !showControlHints;
                    // Set help message, if help message is turned on, else cover it up
                    Console.SetCursorPosition(0, theController.theMinefield.Height + 4);
                    if (showControlHints)
                        Console.WriteLine(helpMessage);
                    else // cover the message up
                    {
                        for (int i = 0; i < helpMessage.Length; i++)
                            Console.Write(" "); // would be more efficient to use a StringBuilder but the message is short enough that it doesn't matter
                    }

                    // set cursor back to the top left
                    Console.SetCursorPosition(0, 0);
                    break;
            }
        }

        /// <summary>
        /// Event handler for when some tiles need to be visually updated.
        /// </summary>
        /// <param name="updatedTiles"></param>
        private void HandleBoardUpdate(ISet<Tile> updatedTiles)
        {
            VisuallyUpdateTiles(updatedTiles);
        }

        /// <summary>
        /// Takes a list of updated cells and updates them visually in the game grid. 
        /// </summary>
        /// <param name="tilesToUpdate"></param>
        private void VisuallyUpdateTiles(ISet<Tile> tilesToUpdate)
        {
            // Set indicator message
            Console.SetCursorPosition(0, theController.theMinefield.Height + 3);
            Console.ForegroundColor = accentColor;
            // cover up the previous message if it was longer
            Console.WriteLine(theController.statusMessage + "                                                                                          ");
            Console.ResetColor();


            foreach (Tile t in tilesToUpdate)
            {
                Console.SetCursorPosition(t.Col * 2 + 4, t.Row + 2);

                // invert colors to show which tile is selected
                if (t.IsSelected)
                    Console.Write("\u001b[7m");

                if (t.IsFlagged)
                    Console.Write("▲ ");
                else if (t.IsHidden)
                    Console.Write("\u001b[37m░░");
                else if (t.IsMine)
                    Console.Write("҉ ");
                else
                {
                    switch (t.Value)
                    {
                        case 0:
                            Console.Write("  ");
                            break;
                        case 1:
                            Console.Write("\u001b[34m" + t.Value + " "); // blue
                            break;
                        case 2:
                            Console.Write("\u001b[32m" + t.Value + " "); // green
                            break;
                        case 3:
                            Console.Write("\u001b[31m" + t.Value + " "); // red
                            break;
                        case 4:
                            Console.Write("\u001b[33m" + t.Value + " "); //yellow
                            break;
                        default:
                            Console.Write("\u001b[35m" + t.Value + " "); //magenta
                            break;
                    }
                }
                Console.Write("\u001b[0m");
            }

            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Prints the gridlines, numbers, and letters for the game board and then starts the main game loop
        /// </summary>
        private void HandleInitBoard()
        {
            // print letter guides
            StringBuilder sb = new StringBuilder();
            sb.Append("    ");
            for (int i = 0; i < theController.theMinefield.Width; i++)
                sb.Append($"{(char)('a' + i % 26)} ");

            sb.Append("\r\n");

            sb.Append("    _");
            for (int i = 0; i < theController.theMinefield.Width * 2; i++)
                sb.Append("_");

            sb.Append("\r\n");

            // done appending top border, now go down the left edge and append line numbers

            for (int row = 0; row < theController.theMinefield.Height; row++)
            {
                sb.Append((row + 1));

                // print white space to align left border regardless of number of digits
                for (int i = 0; i < (3 - (row + 1).ToString().Length); i++)
                    sb.Append(" ");

                sb.Append("|");

                // append end of line
                sb.Append("\r\n");

            }

            // Print the whole thing all at once
            Console.WriteLine(sb.ToString());

            // print control hint
            Console.SetCursorPosition(0, theController.theMinefield.Height + 4);
            Console.WriteLine(helpMessage);

            // start the main game loop
            while (true) // should still exit when user presses normal console exit key combo
                ReadInput();
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

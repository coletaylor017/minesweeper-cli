using System;

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
            string message;
            if (gameIsWon)
            {
                message = "You win! Play again (y/n)?";
            }
            else
            {
                // reveal all tiles
                theController.theMinefield.RevealAllTiles();
                PrintWorld();
                message = "You lost :( Play again (y/n)";
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
            
        }
        
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

            while (input != "quit" && input != "q" && input != "exit" && input != "stop")
            {
                PrintWorld();
                input = Console.ReadLine();
                    
                // TODO: input validation

                if (input.Substring(0, 3) == "dig")

                {
                    int colNum = input[4] - 'a'; // convert char to int
                    int rowNum = rowNum = int.Parse(input.Substring(5)) - 1;
                    theController.Dig(colNum, rowNum);
                }
                else if (input.Substring(0, 4) == "flag")
                {
                    int colNum = input[5] - 'a'; // convert char to int
                    int rowNum = int.Parse(input.Substring(6)) - 1;
                    theController.ToggleFlag(colNum, rowNum);
                }
                else
                {
                    Console.WriteLine("Unrecognized command.");
                }
            }
        }

        private void PrintWorld()
        {
            Console.Clear();
            // print letter guides
            // Should use C# equivalent to Java StringBuilder in next version
            string row = "   | ";
            for (int i = 0; i < theController.theMinefield.Width; i++)
                row += $"{(char)('a' + i)} ";

            Console.WriteLine(row);

            row = "___|_";
            for (int i = 0; i < theController.theMinefield.Width * 2; i++)
                row += "_";

            Console.WriteLine(row);

            // data is stored in columns of rows, but we have to get it to rows of columns in order to print it out
            for (int j = 0; j < theController.theMinefield.Height; j++)
            {
                Console.Write((j + 1));

                // print white space to align left border regardless of number of digits
                for (int n = 0; n < (3 - (j + 1).ToString().Length); n++)
                    Console.Write(" ");

                Console.Write("| ");

                // ditto about using stringBuilder equivalent here
                row = "";

                for (int i = 0; i < theController.theMinefield.Width; i++)
                {
                    Tile t = theController.theMinefield.GetTile(i, j);
                    if (t.IsFlagged)
                        row += "f ";
                    else if (t.IsHidden)
                        row += "- ";
                    else if (t.IsMine)
                        row += "M ";
                    else if (t.Value == 0)
                        row += "  ";
                    else
                        row += t.Value + " ";
                }
                Console.WriteLine(row);
            }
        }
    }
}

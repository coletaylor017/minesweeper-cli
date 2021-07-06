using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    /// <summary>
    /// The game controller. Provides methods to the view that take user inputs and update the model, and returns game state to the view via returning values in these methods. 
    /// </summary>
    public class GameController
    {
        /// <summary>
        /// The instance of the game model
        /// </summary>
        public Minefield theMinefield;

        /// <summary>
        /// Fires when a player has dug up all non-mine spaces or when they dig up a mine. Boolean is true for a win, false for a loss
        /// </summary>
        public event Action<bool> GameOver;

        /// <summary>
        /// Fires whenever the board has been updated, meaning the cursor has been moved or a group of tiles has been revealed or otherwise changed.
        /// The Set of Tiles indicates the grid squares that need to be visually updated as a result of this change.
        /// </summary>
        public event Action<ISet<Tile>> BoardUpdated;

        /// <summary>
        /// Signals that the board needs to be visually initialized with grid borders and labels
        /// </summary>
        public event Action InitBoard;

        /// <summary>
        /// The status message to display below the game board
        /// </summary>
        public string statusMessage = "Selected: a1";

        /// <summary>
        /// The X position of the cursor. 
        /// </summary>
        public int CursorX { get { return theMinefield.SelectedCol; } }

        /// <summary>
        /// The vertical position of the cursor. 
        /// </summary>
        public int CursorY { get { return theMinefield.SelectedRow; } }

        /// <summary>
        /// Move the current tile selection up one square
        /// </summary>
        public void MoveCursorUp()
        {
            // keep track of the previously selected tile bc it will need to be visually updated when it is deselected
            HashSet<Tile> prevTile = new HashSet<Tile>();
            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));

            if (theMinefield.SelectedRow > 0)
                theMinefield.SelectedRow--;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);

            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));
            // indicate that the cursor and status message, as well as the previously selected tile, need to be visually updated
            BoardUpdated(prevTile);
        }

        /// <summary>
        /// Move the current tile selection down one square
        /// </summary>
        public void MoveCursorDown()
        {
            // keep track of the previously selected tile bc it will need to be visually updated when it is deselected
            HashSet<Tile> prevTile = new HashSet<Tile>();
            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));

            if (theMinefield.SelectedRow < theMinefield.Height - 1)
                theMinefield.SelectedRow++;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);

            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));
            
            // indicate that the cursor and status message, as well as the previously selected tile, need to be visually updated
            BoardUpdated(prevTile);
        }

        /// <summary>
        /// Move the current tile selection left one square
        /// </summary>
        public void MoveCursorLeft()
        {
            // keep track of the previously selected tile bc it will need to be visually updated when it is deselected
            HashSet<Tile> prevTile = new HashSet<Tile>();
            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));

            if (theMinefield.SelectedCol > 0)
                theMinefield.SelectedCol--;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);

            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));
            // indicate that the cursor and status message, as well as the previously selected tile, need to be visually updated
            BoardUpdated(prevTile);
        }

        /// <summary>
        /// Move the current tile selection right one square
        /// </summary>
        public void MoveCursorRight()
        {
            // keep track of the previously selected tile bc it will need to be visually updated when it is deselected
            HashSet<Tile> prevTile = new HashSet<Tile>();
            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));

            if (theMinefield.SelectedCol < theMinefield.Width - 1)
                theMinefield.SelectedCol++;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);

            prevTile.Add(theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow));
            // indicate that the cursor and status message, as well as the previously selected tile, need to be visually updated
            BoardUpdated(prevTile);
        }

        public void NewGame(int width, int height, int numMines)
        {
            theMinefield = new Minefield(width, height, numMines);
            // signal to the View that all grid squares need to be rendered
            HashSet<Tile> allTiles = new HashSet<Tile>();
            foreach (Tile t in theMinefield.IterateAllTiles())
                allTiles.Add(t);

            BoardUpdated(allTiles);
            InitBoard();
        }

        /// <summary>
        /// Digs the currently selected tile.
        /// </summary>
        public void Dig()
        {
            Dig(theMinefield.SelectedCol, theMinefield.SelectedRow);
        }

        /// <summary>
        /// Digs (reveals) the selected grid square
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public ISet<Tile> Dig(int col, int row)
        {
            if (theMinefield.GetTile(col, row).IsFlagged)
            {
                statusMessage = "You must remove the flag before digging that space";
                return new HashSet<Tile>();
            }

            ISet<Tile> revealedTiles = theMinefield.Dig(col, row);
            BoardUpdated(revealedTiles);
            if (theMinefield.GetTile(col, row).IsMine) // if it was a mine
                GameOver(false); // they lost

            bool hasWon = true;
            // check if the user has won
            foreach (Tile t in theMinefield.IterateAllTiles())
            {
                // every tile should be either revealed or a mine
                if (t.IsHidden && !t.IsMine)
                {
                    hasWon = false;
                }
            }

            if (hasWon)
                GameOver(true); // they won

            return revealedTiles;
        }

        /// <summary>
        /// Flags or unflags the currently selected square.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public void ToggleFlag()
        {
            if (theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow).IsHidden)
            {
                theMinefield.ToggleFlag(theMinefield.SelectedCol, theMinefield.SelectedRow);
                if (theMinefield.GetTile(theMinefield.SelectedCol, theMinefield.SelectedRow).IsFlagged)
                    statusMessage = "The square is now flagged, this prevents you from accidentally digging it";
                else
                    statusMessage = "The square is now un-flagged";
            }
            else
                statusMessage = "You cannot flag an already revealed space";
        }

        /// <summary>
        /// Flags or unflags a particular square. 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public void ToggleFlag(int col, int row)
        {
            theMinefield.ToggleFlag(col, row);
        }
        
    }
}

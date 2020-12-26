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
        /// The status message to display below the game board
        /// </summary>
        public string statusMessage = "Selected: a1";

        /// <summary>
        /// Move the current tile selection up one square
        /// </summary>
        public void MoveCursorUp()
        {
            if (theMinefield.SelectedRow > 0)
                theMinefield.SelectedRow--;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);
        }

        /// <summary>
        /// Move the current tile selection down one square
        /// </summary>
        public void MoveCursorDown()
        {
            if (theMinefield.SelectedRow < theMinefield.Height - 1)
                theMinefield.SelectedRow++;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);
        }

        /// <summary>
        /// Move the current tile selection left one square
        /// </summary>
        public void MoveCursorLeft()
        {
            if (theMinefield.SelectedCol > 0)
                theMinefield.SelectedCol--;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);
        }

        /// <summary>
        /// Move the current tile selection right one square
        /// </summary>
        public void MoveCursorRight()
        {
            if (theMinefield.SelectedCol < theMinefield.Width - 1)
                theMinefield.SelectedCol++;

            statusMessage = "Selected: " + (char)('a' + theMinefield.SelectedCol) + (theMinefield.SelectedRow + 1);
        }

        public void NewGame(int width, int height, int numMines)
        {
            theMinefield = new Minefield(width, height, numMines);
        }

        /// <summary>
        /// Digs the currently selected tile.
        /// </summary>
        public void Dig()
        {
            Dig(theMinefield.SelectedCol, theMinefield.SelectedRow);
        }

        public void Dig(int col, int row)
        {
            if (theMinefield.GetTile(col, row).IsFlagged)
            {
                statusMessage = "You must remove the flag before digging that space";
                return;
            }

            if (theMinefield.Dig(col, row)) // if it was a mine
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

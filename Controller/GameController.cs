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

        public void NewGame(int width, int height, int numMines)
        {
            theMinefield = new Minefield(width, height, numMines);
        }

        public void Dig(int col, int row)
        {
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

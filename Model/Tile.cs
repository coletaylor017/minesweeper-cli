using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    /// <summary>
    /// Represents a single tile in the world.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Keeps track of whether or not this tile has been visited by a recursive algorithm
        /// </summary>
        public bool Visited { get; set; }

        /// <summary>
        /// Whether or not this tile contains a mine
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// The flag flag!
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Flag for whether or not this tile should be visible to the user
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Flag for whether or not this square is currently selected by the user.
        /// </summary>
        public bool IsSelected { get; set; }

        public int Col { get; private set; }

        public int Row { get; private set; }

        /// <summary>
        /// The number on the square that indicates the number of mine-containing neighbors. 
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Constructs a new tile with the given coordinates that is hidden, not a mine, and not flagged. 
        /// </summary>
        public Tile(int col, int row)
        {
            Row = row;
            Col = col;
            IsMine = false;
            IsFlagged = false;
            IsHidden = true;
            IsSelected = false;
        }
    }
}

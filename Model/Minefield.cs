using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    /// <summary>
    /// The world model for a minesweeper instance. Keeps the state of the minefield grid and all its contents.
    /// </summary>
    public class Minefield
    {
        /// <summary>
        /// The 2D array representing the entire minefield. 
        /// </summary>
        private List<List<Tile>> field;

        private Random rng = new Random();

        public int Width { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Generates a new valid minesweeper setup with the given width, height, and number of mines
        /// </summary>
        public Minefield(int width, int height, int numMines)
        {
            Width = width;
            Height = height;

            field = new List<List<Tile>>();

            // populate with empty mine-less tiles
            for (int i = 0; i < width; i++)
            {
                field.Add(new List<Tile>());
                for (int j = 0; j < height; j++)
                    field[i].Add(new Tile(i, j));
            }

            // set random mines
            int minesPlaced = 0;
            while(minesPlaced < numMines)
            {
                Tile t = GetTile(rng.Next(width), rng.Next(height));
                if (!t.IsMine)
                {
                    t.IsMine = true;
                    minesPlaced++;
                }
            }
                

            // set inital values
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Tile t = field[i][j];
                    // if not a mine
                    if (!field[i][j].IsMine)
                        // iterate through neighbors and add up number of mines
                        foreach(Tile neighbor in IterateNeighbors(i, j))
                                if (neighbor.IsMine)
                                    t.Value++;
                }
            }    
        }

        /// <summary>
        /// Flags or unflags the tile at the specified coordinates.
        /// </summary>
        public void ToggleFlag(int col, int row)
        {
            field[col][row].IsFlagged = !field[col][row].IsFlagged;
        }

        /// <summary>
        /// Reveals the tile at the specified coordinates as well as all the tiles that are revealed as a result. If it was a mine, returns true, else, returns false. 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool Dig(int col, int row)
        {
            // mark all tiles as unvisited 
            foreach (Tile t in IterateAllTiles())
                t.Visited = false;

            RevealTileAndNeighbors(col, row); // recursively reveal an entire chunk
            field[col][row].IsFlagged = false;
            return field[col][row].IsMine;
        }

        /// <summary>
        /// Convenience method for iterating through all tiles in a field
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public IEnumerable<Tile> IterateAllTiles()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    yield return field[i][j];
        }

        /// <summary>
        /// Gets the tile at the specified row and column.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Tile GetTile(int col, int row)
        {
            return field[col][row];
        }

        /// <summary>
        /// Marks all tiles as non-hidden.
        /// </summary>
        public void RevealAllTiles()
        {
            foreach(Tile t in IterateAllTiles())
            {
                t.IsHidden = false;
            }
        }

        /// <summary>
        /// Helper method for iterating through a tile's neighbors (direct and diagonal for a total of up to eight)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private IEnumerable<Tile> IterateNeighbors(int col, int row)
        {
            for (int i = col - 1; i <= col + 1; i++)
                for (int j = row - 1; j <= row + 1; j++)
                    if (!(i == col && j == row) && i >= 0 && j >= 0 && i < Width && j < Height) // don't want to return the tile itself, and don't try to access if out of field bounds
                        yield return field[i][j];
        }

        /// <summary>
        /// Reveals a tile, and recursively does this on all its neighbors if they are hidden, empty, and unflagged
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private void RevealTileAndNeighbors(int col, int row)
        {
            Tile t = field[col][row];
            t.IsHidden = false;
            t.Visited = true;
            foreach (Tile neighbor in IterateNeighbors(col, row))
            {
                // reveal neighbors
                if (!neighbor.IsMine && !neighbor.IsFlagged)
                    neighbor.IsHidden = false;

                // Only recur when this cell's neighbors are empty, non-flagged, mine-free, and unvisited by this algorithm.
                // Base cases are: neighbor is nonzero, flagged, mine, or has already been visited.
                if (neighbor.Value == 0 && !neighbor.IsMine && !neighbor.IsFlagged && !neighbor.Visited)
                    RevealTileAndNeighbors(neighbor.Col, neighbor.Row);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    /// <summary>
    /// Represents a cell on the board
    /// </summary>
    public class Square
    {
        /// <summary>
        /// Represents the colour of the piece (if any) in the cell 
        /// </summary>
        public Piece Piece { get; set; } = Piece.None;

        /// <summary>
        /// True if the current player could play in this cell
        /// </summary>
        public bool IsValidMove { get; set; } 

        public Square Clone()
        {
            return new Square()
            {
                Piece = this.Piece,
                IsValidMove = this.IsValidMove
            };
        }

        public override string ToString()
        {
            return Piece.ToString();
        }
    }
}

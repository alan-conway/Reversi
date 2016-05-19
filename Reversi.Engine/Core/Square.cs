using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    /// <summary>
    /// Represents a cell on the board
    /// </summary>
    public struct Square
    {
        public Square(Piece piece, bool isValid)
        {
            Piece = piece;
            IsValidMove = isValid;
        }

        /// <summary>
        /// Represents the colour of the piece (if any) in the cell 
        /// </summary>
        public Piece Piece { get; set; }

        /// <summary>
        /// True if the current player could play in this cell
        /// </summary>
        public bool IsValidMove { get; set; } 
        
        public override string ToString()
        {
            return Piece.ToString();
        }
    }
}

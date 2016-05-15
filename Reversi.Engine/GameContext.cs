using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    /// <summary>
    /// Holds the minimal state of the game.
    /// </summary>
    /// <remarks>
    /// Also provides some basic helper methods
    /// </remarks>
    public class GameContext : IGameContext
    {
        public GameContext()
        {
            Initialise();
        }

        public Square[] Squares { get; private set; }

        public int MoveNumber { get; private set; } = 1;

        public Piece CurrentPiece
        {
            get { return MoveNumber % 2 == 0 ? Piece.White : Piece.Black; }
        }

        public Piece EnemyPiece
        {
            get { return MoveNumber % 2 == 0 ? Piece.Black : Piece.White; }
        }

        public void Initialise()
        {
            Squares = Enumerable.Range(0, 64).Select(x => new Square()).ToArray();
            MoveNumber = 1;
        }

        public Square this[int location]
        {
            get { return Squares[location]; }
        } 

        public void SetMovePlayed()
        {
            MoveNumber++;
        }

        public void SetAllInvalid()
        {
            foreach(var sq in Squares)
            {
                sq.IsValidMove = false;
            }
        }

        public GameContext Clone()
        {
            return new GameContext()
            {
                Squares = this.Squares,
                MoveNumber = this.MoveNumber
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Core
{
    /// <summary>
    /// Holds the minimal state of the game.
    /// </summary>
    /// <remarks>
    /// Also provides some basic helper methods
    /// </remarks>
    public class GameContext : IGameContext
    {

        public GameContext() : this(true)
        {
        }

        private GameContext(bool initialise)
        {
            if (initialise)
            {
                Initialise();
            }
        }

        public Square[] Squares { get; private set; }

        public int MoveNumber { get; private set; } = 1;

        public Piece CurrentPiece { get; private set; } = Piece.Black;

        public Piece EnemyPiece { get; private set; } = Piece.White;

        public void SetPiece(int location, Piece piece)
        {
            Squares[location].Piece = piece;
        }

        public void SetValid(int location, bool validity)
        {
            Squares[location].IsValidMove = validity;
        }

        public void Initialise()
        {
            Squares = Enumerable.Range(0, 64)
                .Select(x => new Square(Piece.None, false))
                .ToArray();
            MoveNumber = 1;
            CurrentPiece = Piece.Black;
            EnemyPiece = Piece.White;
        }

        public Square this[int location]
        {
            get { return Squares[location]; }
        } 

        public void SetMovePlayed()
        {
            MoveNumber++;

            var tmp = CurrentPiece;
            CurrentPiece = EnemyPiece;
            EnemyPiece = tmp;
        }

        public void SetAllInvalid()
        {
            for(int i = 0; i < Squares.Length; i++)
            {
                Squares[i].IsValidMove = false;
            }
        }

        public IGameContext Clone()
        {
            var newContext = new GameContext(false)
            {
                Squares = new Square[Squares.Length],                
                MoveNumber = this.MoveNumber,
                CurrentPiece = CurrentPiece,
                EnemyPiece = EnemyPiece
            };
            for(int i = 0; i < Squares.Length; i++)
            {
                newContext.Squares[i] = Squares[i];
            }
            return newContext;
        }
    }
}

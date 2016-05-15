using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Helpers
{
    /// <summary>
    /// Examines the game and decides whether the game has finished or not
    /// </summary>
    public class GameStatusExaminer : IGameStatusExaminer
    {
        private IValidMoveFinder _validMoveFinder;

        public GameStatusExaminer(IValidMoveFinder validMoveFinder)
        {
            _validMoveFinder = validMoveFinder;
        }

        public GameStatus DetermineGameStatus(IGameContext context)
        {
            if (!_validMoveFinder.IsAnyMoveValid(context))
            {
                //create a hypothetical context to model the next move
                var futureContext = context.Clone();
                futureContext.SetMovePlayed();

                if (!_validMoveFinder.IsAnyMoveValid(futureContext))
                {
                    // i.e. neither player can make another valid move
                    int numBlack = context.Squares.Count(s => s.Piece == Piece.Black);
                    int numWhite = context.Squares.Count(s => s.Piece == Piece.White);

                    return (numBlack == numWhite) ? GameStatus.Draw :
                        (numBlack > numWhite) ? GameStatus.BlackWins : GameStatus.WhiteWins;
                }
            }
            return GameStatus.InProgress;
        }
    }
}

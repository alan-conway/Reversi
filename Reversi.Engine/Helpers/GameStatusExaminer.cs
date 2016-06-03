using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Core;

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
                var nextMoveContext = context.Clone();
                nextMoveContext.SetMovePlayed();

                if (!_validMoveFinder.IsAnyMoveValid(nextMoveContext))
                {
                    // i.e. neither player can make another valid move
                    return GetFinalGameStatus(context);
                }
            }
            return GameStatus.InProgress;
        }

        private static GameStatus GetFinalGameStatus(IGameContext context)
        {
            int numBlack = context.Squares.Count(s => s.Piece == Piece.Black);
            int numWhite = context.Squares.Count(s => s.Piece == Piece.White);

            return (numBlack == numWhite) ? GameStatus.Draw :
                (numBlack > numWhite) ? GameStatus.BlackWins : GameStatus.WhiteWins;
        }
    }
}

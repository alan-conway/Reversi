using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Heuristics
{
    /// <summary>
    /// The difference in the player's number of possible moves
    /// </summary>
    public class MobilityHeuristic : IHeuristic
    {
        private IValidMoveFinder _moveFinder;

        public MobilityHeuristic(IValidMoveFinder moveFinder)
        {
            _moveFinder = moveFinder;
        }

        public int GetScore(IGameContext context, Piece relativePiece)
        {
            var playerMobility = _moveFinder.FindAllValidMoves(context, relativePiece).Count();

            var otherPlayer = relativePiece == Piece.Black ? Piece.White : Piece.Black;
            var opponentMobility = _moveFinder.FindAllValidMoves(context, otherPlayer).Count();
            
            return playerMobility - opponentMobility;
        }

    }
}

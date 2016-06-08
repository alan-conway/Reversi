using Game.Search.Interfaces;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax
{
    /// <summary>
    /// Uses heuristics to evaluate a board layout
    /// </summary>
    public class ReversiScoreProvider : IScoreProvider 
    {
        private IHeuristic _winLoseHeuristic;
        private IHeuristic _cornerHeuristic;
        private IHeuristic _mobilityHeuristic;

        public ReversiScoreProvider(IHeuristic winLoseHeuristic, 
            IHeuristic cornerHeuristic, IHeuristic mobilityHeuristic)
        {
            _winLoseHeuristic = winLoseHeuristic;
            _cornerHeuristic = cornerHeuristic;
            _mobilityHeuristic = mobilityHeuristic;
        }

        /// <summary>
        /// Evaluate from point of view of the nominal player
        /// </summary>
        /// <param name="node"></param>
        /// <returns>positive score for if player is ahead, negative if behind</returns>
        public int EvaluateScore(ITreeNode node, bool isPlayer1)
        {
            var context = (node as IReversiTreeNode).Context;
            var relativePiece = isPlayer1 ? Piece.Black : Piece.White; // since black is player1

            var winLoseScore = _winLoseHeuristic.GetScore(context, relativePiece);
            var cornerScore = _cornerHeuristic.GetScore(context, relativePiece);
            var mobilityScore = _mobilityHeuristic.GetScore(context, relativePiece);
            return winLoseScore + cornerScore + mobilityScore;
        }

    }
}

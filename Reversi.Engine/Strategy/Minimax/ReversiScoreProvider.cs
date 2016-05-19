using Game.Search.Interfaces;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Heuristics;
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
        private IGameStatusExaminer _statusExaminer;
        private IHeuristic _cornerHeuristic;
        private IHeuristic _mobilityHeuristic;

        public ReversiScoreProvider(IGameStatusExaminer statusExaminer, 
            IHeuristic cornerHeuristic, IHeuristic mobilityHeuristic)
        {
            _statusExaminer = statusExaminer;
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
            var treeNode = node as IReversiTreeNode;
            var relativePiece = isPlayer1 ? Piece.Black : Piece.White; // since black is player1

            var status = _statusExaminer.DetermineGameStatus(treeNode.Context);

            switch (status)
            {
                case GameStatus.BlackWins: return (isPlayer1 ? 1000 : -1000);
                case GameStatus.WhiteWins: return (isPlayer1 ? -1000 : 1000);
                case GameStatus.NewGame: return 0;
                case GameStatus.Draw: return 0;
                default: return EvaluateGameInProgress(treeNode.Context, relativePiece);
            }
        }

        private int EvaluateGameInProgress(IGameContext context, Piece relativePiece)
        {
            var cornerScore = _cornerHeuristic.GetScore(context, relativePiece);
            var mobilityScore = _mobilityHeuristic.GetScore(context, relativePiece);
            return cornerScore + mobilityScore;
        }

    }
}

using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    public class ScoreCalculator : IScoreCalculator
    {
        /// <summary>
        /// Displays the status (won/drawn) of the game along with the scores
        /// </summary>
        /// <remarks>
        /// At the end of the game, if there is a winner, the empty squares
        /// are awarded, by convention, to the winner
        /// </remarks>
        public void CalculateScores(GameStatus status, Square[] squares, out int blackScore, out int whiteScore)
        {
            blackScore = squares.Count(s => s.Piece == Piece.Black);
            whiteScore = squares.Count(s => s.Piece == Piece.White);

            // convention is to award the empty squares to the victor
            if (status != GameStatus.InProgress)
            {
                int numEmptySquares = squares.Count(s => s.Piece == Piece.None);
                if (blackScore > whiteScore)
                {
                    blackScore += numEmptySquares;
                }
                else if (whiteScore > blackScore)
                {
                    whiteScore += numEmptySquares;
                }
            }
        }
    }
}

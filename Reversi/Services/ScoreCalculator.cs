using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    /// <summary>
    /// Returns the score for each player
    /// </summary>
    /// <remarks>
    /// At the end of the game, if there is a winner, the empty squares
    /// are awarded, by convention, to the winner
    /// </remarks>

    public class ScoreCalculator : IScoreCalculator
    {
        public int CalculateScoreForPlayer(Piece player, GameStatus status, Square[] squares)
        {
            int playerScore = squares.Count(s => s.Piece == player);
            
            if (status != GameStatus.InProgress)
            {
                // convention is to award the empty squares to the victor
                Piece opponentPiece = player == Piece.Black ? Piece.White : Piece.Black;
                int opponentScore = squares.Count(s => s.Piece == opponentPiece);
                if (playerScore > opponentScore)
                {
                    playerScore += squares.Count(s => s.Piece == Piece.None);
                }
            }

            return playerScore;
        }
    }
}

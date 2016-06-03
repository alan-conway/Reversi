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
            int playerScore = CountSquaresMatchingPiece(player, squares);

            if (status == GameStatus.InProgress)
            {
                return playerScore;
            }

            //at the end of the game, award empty squares to the victor
            playerScore += CountEmptySquaresIfWinner(player, squares, playerScore);

            return playerScore;
        }

        private int CountSquaresMatchingPiece(Piece piece, Square[] squares)
        {
            return squares.Count(s => s.Piece == piece);
        }

        private int CountEmptySquaresIfWinner(Piece player, Square[] squares, int playerScore)
        {
            // convention is to award the empty squares to the victor
            Piece opponentPiece = player == Piece.Black ? Piece.White : Piece.Black;
            int opponentScore = CountSquaresMatchingPiece(opponentPiece, squares);
            if (playerScore > opponentScore)
            {
                return CountSquaresMatchingPiece(Piece.None, squares);
            }
            else
            {
                return 0;
            }
        }
    }
}

using Reversi.Engine.Core;

namespace Reversi.Services
{
    /// <summary>
    /// Calculates the scores for black and white according to the layout of the board
    /// </summary>
    public interface IScoreCalculator
    {
        void CalculateScores(GameStatus status, Square[] squares, out int blackScore, out int whiteScore);
    }
}
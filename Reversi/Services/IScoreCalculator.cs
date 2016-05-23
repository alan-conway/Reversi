using Reversi.Engine.Core;

namespace Reversi.Services
{
    /// <summary>
    /// Calculates the scores for black and white according to the layout of the board
    /// </summary>
    public interface IScoreCalculator
    {
        int CalculateScoreForPlayer(Piece player, GameStatus status, Square[] squares);
    }        
}
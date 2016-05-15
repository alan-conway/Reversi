namespace Reversi.Engine.Interfaces
{
    /// <summary>
    /// Decides whether a game is still in progress, or has ended
    /// </summary>
    public interface IGameStatusExaminer
    {
        /// <summary>
        /// Determines whether a game has finished or not from the state
        /// of the board
        /// </summary>
        GameStatus DetermineGameStatus(IGameContext context);
    }
}
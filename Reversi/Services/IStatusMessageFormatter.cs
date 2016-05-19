using Reversi.Engine.Core;

namespace Reversi.Services
{
    /// <summary>
    /// Formats the status message to display, depending on the state of the game
    /// </summary>
    public interface IStatusMessageFormatter
    {
        string GetStatusMessage(GameStatus status, Square[] squares);
    }
}
using Reversi.Engine.Core;

namespace Reversi.Engine.Interfaces
{
    /// <summary>
    /// Represents the status of the game
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// The state of the board
        /// </summary>
        Square[] Squares { get; }

        /// <summary>
        /// This shows the number of the current move
        /// </summary>
        int MoveNumber { get; }

        /// <summary>
        /// The piece that will make the current move
        /// </summary>
        Piece CurrentPiece { get; }

        /// <summary>
        /// The enemy of the current piece
        /// </summary>
        Piece EnemyPiece { get; }

        /// <summary>
        /// Gets the square at the location specified
        /// </summary>
        Square this[int location] { get; }

        /// <summary>
        /// Sets the piece at the given location
        /// </summary>
        void SetPiece(int location, Piece piece);

        /// <summary>
        /// Sets the validity at the given location
        /// </summary>
        void SetValid(int location, bool validity);

        /// <summary>
        /// Performs initialisation to begin a new game
        /// </summary>
        void Initialise();

        /// <summary>
        /// Indicates that a move is over and play moves to the other player
        /// </summary>
        void SetMovePlayed();

        /// <summary>
        /// Clears any IsValidMove flags from all squares
        /// </summary>
        void SetAllInvalid();

        /// <summary>
        /// Creates a clone of the current game context
        /// </summary>
        /// <returns></returns>
        IGameContext Clone();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    public interface IGameEngine
    {
        /// <summary>
        /// The minimal state of the game
        /// </summary>
        IGameContext Context { get; }

        /// <summary>
        /// Clears the board and any internal state and creates a new game
        /// </summary>
        /// <returns>The board in new-game setup</returns>
        Response CreateNewGame();

        /// <summary>
        /// Returns (asynchronously) an updated board with the specified move
        /// </summary>
        /// <param name="move">The move made by the opponent</param>
        /// <returns>The board with enemy pieces approproately captured following the move</returns>
        Task<Response> UpdateBoardAsync(Move move);

        /// <summary>
        /// Returns (asynchronously) an updated board with the engine's move
        /// </summary>
        /// <returns>The board with enemy pieces approproately captured following the move</returns>
        Task<Response> MakeReplyMoveAsync();

        /// <summary>
        /// The number of the current move
        /// </summary>
        int MoveNumber { get; }


    }
}

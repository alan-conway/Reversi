using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;

namespace Reversi.Engine.Interfaces
{
    public interface IGameEngine
    {
        /// <summary>
        /// The minimal state of the game
        /// </summary>
        IGameContext Context { get; }

        /// <summary>
        /// Gets or sets game preferences 
        /// </summary>
        IGameOptions GameOptions { get; set; }

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
        Task<Response> UpdateBoardWithMoveAsync(Move move);

        /// <summary>
        /// Returns (synchronously) an updated board with the specified move
        /// </summary>
        /// <param name="move">The move made by the opponent</param>
        /// <param name="context">The context to update with the result of the move</param>
        /// <returns>The board with enemy pieces approproately captured following the move</returns>
        Response UpdateBoardWithMove(Move move, IGameContext context);

        /// <summary>
        /// Returns (asynchronously) an updated board with the engine's move
        /// </summary>
        /// <returns>The board with enemy pieces approproately captured following the move</returns>
        Task<Response> MakeReplyMoveAsync();

        /// <summary>
        /// Returns (synchronously) an updated board with the engine's move
        /// </summary>
        /// <param name="context">The context to update with the result of the move</param>
        /// <returns>The board with enemy pieces approproately captured following the move</returns>
        Response MakeReplyMove(IGameContext context);

        /// <summary>
        /// The number of the current move
        /// </summary>
        int MoveNumber { get; }


    }
}

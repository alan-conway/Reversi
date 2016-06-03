using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Helpers
{
    /// <summary>
    /// Plays a move by setting a piece in the game context and 
    /// capturing enemy pieces
    /// </summary>
    public class MovePlayer : IMovePlayer
    {
        private ICaptureHelper _captureHelper;
        private IGameStatusExaminer _statusExaminer;

        public MovePlayer(ICaptureHelper captureHelper, IGameStatusExaminer statusExaminer)
        {
            _captureHelper = captureHelper;
            _statusExaminer = statusExaminer;
        }

        /// <summary>
        /// Play the move, capture the enemy pieces and return the 
        /// resulting status of the game
        /// </summary>
        /// <returns>
        /// A MoveResult encapsulating the GameStatus and the GameContext
        /// </returns>
        public MoveResult PlayMove(Move move, IGameContext moveContext)
        {
            if (!move.Pass)
            {
                moveContext.SetPiece(move.LocationPlayed, moveContext.CurrentPiece);
                _captureHelper.CaptureEnemyPieces(moveContext, move.LocationPlayed);
            }

            moveContext.SetAllInvalid();
            moveContext.SetMovePlayed();

            return new MoveResult(_statusExaminer.DetermineGameStatus(moveContext), moveContext);
        }
    }
}

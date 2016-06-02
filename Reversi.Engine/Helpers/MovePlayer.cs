using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Helpers
{
    public class MovePlayer : IMovePlayer
    {
        private ICaptureHelper _captureHelper;
        private IGameStatusExaminer _statusExaminer;

        public MovePlayer(ICaptureHelper captureHelper, IGameStatusExaminer statusExaminer)
        {
            _captureHelper = captureHelper;
            _statusExaminer = statusExaminer;
        }

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

using Reversi.Engine.Strategy.Minimax.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Heuristics
{
    public class WinLoseHeuristic : IHeuristic
    {
        private IGameStatusExaminer _statusExaminer;

        public WinLoseHeuristic(IGameStatusExaminer statusExaminer)
        {
            _statusExaminer = statusExaminer;
        }

        public int GetScore(IGameContext context, Piece relativePiece)
        {
            var status = _statusExaminer.DetermineGameStatus(context);

            switch (status)
            {
                case GameStatus.BlackWins: return (relativePiece == Piece.Black ? 1000 : -1000);
                case GameStatus.WhiteWins: return (relativePiece == Piece.White ? 1000 : -1000);
                default: return 0;
            }
        }
    }
}

using Game.Search.Interfaces;
using Game.Search.MonteCarlo;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.MonteCarlo
{
    public class MonteCarloPlayoutSimulator : ISimulator
    {
        private IMovePlayer _movePlayer;
        private IRandomMoveStrategy _randomMoveStrategy;

        public MonteCarloPlayoutSimulator(IRandomMoveStrategy randomMoveStrategy, 
            IMovePlayer movePlayer)
        {
            _randomMoveStrategy = randomMoveStrategy;
            _movePlayer = movePlayer;
        }

        public int SimulateGameToCompletionFromNode(ITreeNode node)
        {
            var context = (node as IReversiTreeNode).Context.Clone();
            var status = PlayGameToCompletion(context);
            return ScoreGameOutcome(status);
        }

        private GameStatus PlayGameToCompletion(IGameContext context)
        {
            var status = GameStatus.InProgress;
            while (status == GameStatus.InProgress)
            {
                var randomMove = _randomMoveStrategy.ChooseMove(context, null);
                var moveResult = _movePlayer.PlayMove(randomMove, context);
                status = moveResult.Status;
            }
            return status;
        }

        private static int ScoreGameOutcome(GameStatus status)
        {
            return (status == GameStatus.BlackWins) ? 1 :
                   ((status == GameStatus.WhiteWins) ? -1 : 0);
        }
    }
}

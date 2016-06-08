using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;
using Game.Search.MonteCarlo;
using Game.Search.Interfaces;
using Reversi.Engine.Strategy.Shared;

namespace Reversi.Engine.Strategy.MonteCarlo
{
    public class MonteCarloMoveStrategy : IMoveStrategy
    {
        private const string Name = "Monte Carlo";
        private const bool IsMultiLevel = true;
        private const double SearchDurationMultiplier = 0.5;

        private IMonteCarloTreeEvaluator _monteCarlo;
        private IValidMoveFinder _moveFinder;
        private IReversiTreeNodeBuilder _treeNodeBuilder;

        public MonteCarloMoveStrategy(IMonteCarloTreeEvaluator monteCarlo,
            IValidMoveFinder moveFinder, IReversiTreeNodeBuilder treeNodeBuilder)
        {
            _monteCarlo = monteCarlo;
            _moveFinder = moveFinder;
            _treeNodeBuilder = treeNodeBuilder;

            SetLevel(1);
        }

        public StrategyInfo StrategyInfo { get; private set; }

        public double SearchDurationSeconds { get; private set; }

        public void SetLevel(int level)
        {
            SearchDurationSeconds = SearchDurationMultiplier * level;
            StrategyInfo = new StrategyInfo(Name, IsMultiLevel, level);
        }

        public Move ChooseMove(IGameContext context, IMovePlayer movePlayer)
        {
            var validMoves = _moveFinder.FindAllValidMoves(context);
            if (!validMoves.Any())
            {
                return Move.PassMove;
            }
            if (validMoves.Count() == 1)
            {
                return new Move(validMoves.First());
            }
            bool isPlayer1 = context.CurrentPiece == Piece.Black;
            var treeNode = _treeNodeBuilder.CreateRootTreeNode(context, movePlayer);
            var result = _monteCarlo.EvaluateTree(treeNode, isPlayer1, SearchDurationSeconds);
            return new Move(((IReversiTreeNode)result.TreeNode).MoveLocation);
        }

     
    }
}

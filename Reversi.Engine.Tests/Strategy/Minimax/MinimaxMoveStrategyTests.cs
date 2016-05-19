using Game.Search.Minimax;
using Moq;
using Reversi.Engine.Core;
using Reversi.Engine.Helpers;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using Reversi.Engine.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Minimax
{
    public class MinimaxMoveStrategyTests
    {
        private IGameContext _context;
        private IGameEngine _engine;
        private IValidMoveFinder _moveFinder;
        private IGameStatusExaminer _statusExaminer;

        public MinimaxMoveStrategyTests()
        {
            _context = new GameContext();
            var locationHelper = new LocationHelper();
            var captureHelper = new CaptureHelper(locationHelper);
            _moveFinder = new ValidMoveFinder(locationHelper);
            var mockMoveStrategy = new Mock<IMoveStrategy>().Object;
            _statusExaminer = new GameStatusExaminer(_moveFinder);
            _engine = new GameEngine(_context, captureHelper, _moveFinder,
                mockMoveStrategy, _statusExaminer);
            _engine.CreateNewGame();
        }

        private MinimaxMoveStrategy CreateStrategy(IHeuristic cornerHeuristic,
            IHeuristic mobilityHeuristic)
        {
            var minimax = new MinimaxTreeEvaluator();
            var scoreProvider = new ReversiScoreProvider(_statusExaminer,
                cornerHeuristic, mobilityHeuristic);
            var moveOrdering = new MoveOrdering();
            var treeNodeBuilder = new ReversiTreeNodeBuilder(_moveFinder, moveOrdering);
            return new MinimaxMoveStrategy(minimax, _moveFinder, scoreProvider,
                _statusExaminer, treeNodeBuilder, 1);
        }

        [Fact]
        public void ShouldChooseHighestScoringMove()
        {
            //Arrange
            //more of an integration test:
            _context.SetPiece(19, Piece.Black);
            _context.SetPiece(27, Piece.Black);
            _context.SetMovePlayed();

            var cornerHeuristic = new CornerHeuristic();
            var mobilityHeuristic = new MobilityHeuristic(_moveFinder);

            var strategy = CreateStrategy(cornerHeuristic, mobilityHeuristic);

            //Act
            var move = strategy.ChooseMove(_context, _engine);

            //Assert 
            Assert.Equal(18, move.LocationPlayed); // 18 gives white greatest mobility
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Search.Interfaces;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Game.Search.Minimax;
using Reversi.Engine.Core;
using Reversi.Engine.Helpers;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using Reversi.Engine.Strategy.Minimax.Interfaces;
using Xunit;
using Ploeh.AutoFixture.Xunit2;

namespace Reversi.Engine.Tests.Strategy.Minimax
{
    public class MinimaxMoveStrategyTests
    {
        [Fact]
        public void ShouldChooseHighestScoringMove()
        {
            //Arrange
            //more of an integration test:
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<ILocationHelper>(fixture.Freeze<LocationHelper>());
            fixture.Inject<ICaptureHelper>(fixture.Create<CaptureHelper>());
            fixture.Inject<IMoveOrdering>(fixture.Create<MoveOrdering>());
            fixture.Inject<IGameContext>(fixture.Create<GameContext>());
            fixture.Inject<IValidMoveFinder>(fixture.Create<ValidMoveFinder>());
            fixture.Inject<IGameStatusExaminer>(fixture.Create<GameStatusExaminer>());

            var moveFinder = fixture.Freeze<ValidMoveFinder>();
            var statusExaminer = fixture.Freeze<GameStatusExaminer>();

            var winLoseHeuristic = fixture.Create<WinLoseHeuristic>();
            var cornerHeuristic = fixture.Create<CornerHeuristic>();
            var mobilityHeuristic = fixture.Create<MobilityHeuristic>();

            var treeNodeBuilder = fixture.Create<ReversiTreeNodeBuilder>();
            var minimax = fixture.Create<MinimaxTreeEvaluator>();
            var scoreProvider = new ReversiScoreProvider(winLoseHeuristic,
                cornerHeuristic, mobilityHeuristic);

            var gameOptions = fixture.Create<GameOptions>();
            gameOptions.StrategyLevel = 1;
            fixture.Inject<IGameOptions>(gameOptions);
            var strategy = new MinimaxMoveStrategy(minimax, moveFinder, scoreProvider,
                statusExaminer, treeNodeBuilder);

            fixture.Register<IMoveStrategy>(() => strategy);
            var engine = fixture.Create<GameEngine>();
            engine.CreateNewGame();

            engine.Context.SetPiece(19, Piece.Black);
            engine.Context.SetPiece(27, Piece.Black);
            engine.Context.SetMovePlayed();


            //Act
            var move = strategy.ChooseMove(engine.Context, engine);

            //Assert 
            Assert.Equal(18, move.LocationPlayed); // 18 gives white greatest mobility
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(8)]
        public void ShouldSetSearchDepthWhenChangingLevel(int level)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var strategy = fixture.Create<MinimaxMoveStrategy>();

            //Act
            strategy.SetLevel(level);

            //Assert
            Assert.Equal(level, strategy.MaxSearchDepth);
        }
    }
}

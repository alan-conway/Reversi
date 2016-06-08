using Game.Search.Interfaces;
using Game.Search.MonteCarlo;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.MonteCarlo;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.MonteCarlo
{
    public class MonteCarloMoveStrategyTests
    {
        [Fact]
        public void ShouldEvaluateTreeWhenChoosingBetweenMoves()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockContext = fixture.Freeze<Mock<IGameContext>>();
            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(It.IsAny<IGameContext>())).Returns(new[] { 0, 1, 2 });
            var resultNode = fixture.Create<Mock<IReversiTreeNode>>();
            resultNode.Setup(rn => rn.MoveLocation).Returns(1);
            var mockTreeEvaluator = fixture.Freeze<Mock<IMonteCarloTreeEvaluator>>();
            mockTreeEvaluator.Setup(te => te.EvaluateTree(It.IsAny<ITreeNode>(), It.IsAny<bool>(), It.IsAny<double>()))
                .Returns(new MonteCarloResult(10, 10, resultNode.Object));
                
            var strategy = fixture.Create<MonteCarloMoveStrategy>();
            var expectedMoveLocation = 1;

            //Act
            var move = strategy.ChooseMove(mockContext.Object, mockMovePlayer.Object);

            //Assert
            Assert.Equal(expectedMoveLocation, move.LocationPlayed);
            mockTreeEvaluator.Verify(te => te.EvaluateTree(It.IsAny<ITreeNode>(), It.IsAny<bool>(), It.IsAny<double>()), Times.Once);
        }

        [Fact]
        public void ShouldNotEvaluateTreeWhenOnlyOneMoveAvailable()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockContext = fixture.Freeze<Mock<IGameContext>>();
            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(It.IsAny<IGameContext>())).Returns(new[] { 1 });

            var mockTreeEvaluator = fixture.Freeze<Mock<IMonteCarloTreeEvaluator>>();
            
            var strategy = fixture.Create<MonteCarloMoveStrategy>();
            var expectedMoveLocation = 1;

            //Act
            var move = strategy.ChooseMove(mockContext.Object, mockMovePlayer.Object);

            //Assert
            Assert.Equal(expectedMoveLocation, move.LocationPlayed);
            mockTreeEvaluator.Verify(te => te.EvaluateTree(It.IsAny<ITreeNode>(), It.IsAny<bool>(), It.IsAny<double>()), Times.Never);
        }

        [Fact]
        public void ShouldPassWhenNoMovesAreAvailable()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockContext = fixture.Freeze<Mock<IGameContext>>();
            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(It.IsAny<IGameContext>())).Returns(new int[0]);
            var strategy = fixture.Create<MonteCarloMoveStrategy>();

            //Act
            var move = strategy.ChooseMove(mockContext.Object, mockMovePlayer.Object);

            //Assert
            Assert.True(move.Pass);
        }

        [Theory]
        [InlineData(1, 0.5)]
        [InlineData(5, 2.5)]
        public void ShouldSetSearchDurationWhenChangingLevel(int level, double duration)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var strategy = fixture.Create<MonteCarloMoveStrategy>();

            //Act
            strategy.SetLevel(level);

            //Assert
            Assert.Equal(duration, strategy.SearchDurationSeconds);
        }

    }
}

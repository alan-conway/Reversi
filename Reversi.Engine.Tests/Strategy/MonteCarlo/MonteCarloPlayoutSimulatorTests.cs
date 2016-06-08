using Game.Search.Interfaces;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Shared;
using Reversi.Engine.Strategy.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.MonteCarlo
{
    public class MonteCarloPlayoutSimulatorTests
    {
        [Theory]
        [InlineData(GameStatus.BlackWins, 1)]
        [InlineData(GameStatus.WhiteWins, -1)]
        [InlineData(GameStatus.Draw, 0)]
        public void ShouldReturnScoreForSimulatedOutcome(GameStatus outcome, int expectedScore)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockContext = fixture.Freeze<Mock<IGameContext>>();
            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(outcome, mockContext.Object));
            var mockMoveStrategy = fixture.Freeze<Mock<IRandomMoveStrategy>>();
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()))
                .Returns(new Move(50));
            var mockTreeNode = fixture.Create<Mock<IReversiTreeNode>>();
            mockTreeNode.Setup(tn => tn.Context).Returns(mockContext.Object);

            var simulator = fixture.Create<MonteCarloPlayoutSimulator>();

            //Act
            var score = simulator.SimulateGameToCompletionFromNode(mockTreeNode.Object);

            //Assert
            Assert.Equal(expectedScore, score);
            mockMoveStrategy.Verify(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()), Times.AtLeastOnce);
            mockMovePlayer.Verify(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()), Times.AtLeastOnce);
        }
    }
}

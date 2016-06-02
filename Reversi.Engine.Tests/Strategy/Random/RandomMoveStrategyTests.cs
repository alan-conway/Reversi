using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit2;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy;
using Reversi.Engine.Strategy.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Random
{
    public class RandomMoveStrategyTests
    {
        private Mock<IRandomiser> _mockRandom;
        private Mock<IValidMoveFinder> _mockValidMoveFinder;
        private RandomMoveStrategy _strategy;

        public RandomMoveStrategyTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockRandom = fixture.Freeze<Mock<IRandomiser>>();
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            _mockValidMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            _strategy = fixture.Create<RandomMoveStrategy>();
        }

        [Theory]
        [InlineAutoData(true)]
        [InlineAutoData(false)]
        public void ShouldReturnPassOnlyIfNoValidMovesAreAvailable(bool areMovesAvailable,
            Mock<IGameContext> context, Mock<IMovePlayer> movePlayer)
        {
            //Arrange            
            _mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(It.IsAny<IGameContext>()))
                .Returns(areMovesAvailable ? new[] { 0, 1, 2 } : new int[0]);
            bool expectMoveToBePass = !areMovesAvailable;

            //Act
            var move = _strategy.ChooseMove(context.Object, movePlayer.Object);

            //Assert
            Assert.Equal(expectMoveToBePass, move.Pass);
        }

        [Theory]
        [InlineAutoData()]
        public void ShouldReturnReturnMovesRandomly(Mock<IGameContext> context, Mock<IMovePlayer> movePlayer)
        {
            //Arrange
            _mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(It.IsAny<IGameContext>()))
                .Returns(new[] { 0, 1, 2 });

            //Act
            _strategy.ChooseMove(context.Object, movePlayer.Object);

            //Assert - check that we fetch a random number
            _mockRandom.Verify(r => r.Next(It.IsAny<int>(), It.IsAny<int>()),
                Times.Once());
        }
    }
}

using Moq;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy
{
    public class RandomMoveStrategyTests
    {
        private IGameContext _context;
        private IGameEngine _engine;
        private Mock<IRandomiser> _mockRandom;
        private Mock<IValidMoveFinder> _mockValidMoveFinder;
        private RandomMoveStrategy _strategy;

        public RandomMoveStrategyTests()
        {
            _mockRandom = new Mock<IRandomiser>();
            _mockRandom.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);
            _engine = new Mock<IGameEngine>().Object;
            _context = new Mock<IGameContext>().Object;
            _mockValidMoveFinder = new Mock<IValidMoveFinder>();
            _strategy = new RandomMoveStrategy(_mockRandom.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldReturnPassOnlyIfNoValidMovesAreAvailable(bool areMovesAvailable)
        {
            //Arrange            
            _mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(_context))
                .Returns(areMovesAvailable ? new[] { 0, 1, 2 } : new int[0]);
            bool expectMoveToBePass = !areMovesAvailable;

            //Act
            var move = _strategy.ChooseMove(_engine, _context, 
                _mockValidMoveFinder.Object);

            //Assert
            Assert.Equal(expectMoveToBePass, move.Pass);
        }

        [Fact]
        public void ShouldReturnReturnMovesRandomly()
        {
            //Arrange
            _mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(_context))
                .Returns(new[] { 0, 1, 2 });

            //Act
            _strategy.ChooseMove(_engine, _context, _mockValidMoveFinder.Object);

            //Assert - check that we fetch a random number
            _mockRandom.Verify(r => r.Next(It.IsAny<int>(), It.IsAny<int>()), 
                Times.Once);
        }
    }
}

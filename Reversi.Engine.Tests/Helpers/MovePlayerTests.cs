using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Engine.Helpers;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Helpers
{
    public class MovePlayerTests
    {
        private Mock<IGameContext> _mockContext;
        private MovePlayer _movePlayer;

        public MovePlayerTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockContext = fixture.Create<Mock<IGameContext>>();
            var mockStatusExaminer = fixture.Freeze<Mock<IGameStatusExaminer>>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(GameStatus.InProgress);
            _movePlayer = fixture.Create<MovePlayer>();
        }

        [Fact]
        public void ShouldPlayMoveInContextSupplied()
        {
            //Arrange            
            var move = new Move(1);

            //Act
            _movePlayer.PlayMove(move, _mockContext.Object);

            //Assert
            _mockContext.Verify(c => c.SetPiece(1, It.IsAny<Piece>()), Times.Once);
        }

        [Fact]
        public void ShouldIndicateMovePlayedAfterPlayingMove()
        {
            //Arrange
            var move = new Move(1);

            //Act
            _movePlayer.PlayMove(move, _mockContext.Object);

            //Assert
            _mockContext.Verify(c => c.SetMovePlayed(), Times.Once);
        }

        [Fact]
        public void ShouldIncreaseMoveNumberAfterPlayingMove()
        {
            //Arrange
            var move = new Move(1);
            var context = new GameContext();

            //Act
            _movePlayer.PlayMove(move, context);

            //Assert
            Assert.Equal(2, context.MoveNumber);
        }

        [Fact]
        public void ShouldReturnTheReportedStatus()
        {
            //Arrange
            var move = new Move(1);

            //Act
            var result = _movePlayer.PlayMove(move, _mockContext.Object);

            //Assert
            Assert.Equal(GameStatus.InProgress, result.Status);
        }

        [Fact]
        public void ShouldReturnTheSuppliedContext()
        {
            //Arrange
            var move = new Move(1);

            //Act
            var result = _movePlayer.PlayMove(move, _mockContext.Object);

            //Assert
            Assert.Equal(_mockContext.Object, result.Context);
        }

        [Fact]
        public void ShouldSetAllAsInvalidAfterPlayingMove()
        {
            //Arrange            
            var context = new GameContext();
            context.SetValid(0, true);
            context.SetValid(1, true);
            context.SetValid(2, true);
            var move = new Move(1);

            //Act
            var result = _movePlayer.PlayMove(move, context);

            //Assert
            Assert.True(result.Context.Squares.All(s => !s.IsValidMove));
        }
    }
}

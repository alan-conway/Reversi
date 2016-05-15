using Moq;
using Reversi.Engine.Tests.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Xunit;
using Reversi.Engine.Helpers;

namespace Reversi.Engine.Tests
{
    public class GameStatusExaminerTests
    {
        [Theory]
        [InlineData(60, GameStatus.BlackWins)]
        [InlineData(10, GameStatus.WhiteWins)]
        [InlineData(32, GameStatus.Draw)]
        public void ShouldSetCorrectStatusWhenNoValidMovesRemain(int numBlackPieces, 
            GameStatus expectedStatus)
        {
            //Arrange
            int[] blackSquares = Enumerable.Range(0, numBlackPieces-1).ToArray();
            int[] whiteSquares = Enumerable.Range(numBlackPieces, 63-numBlackPieces).ToArray();
            var context = GameContextFactory.CreateGameContext(blackSquares, whiteSquares);
            var mockValidMoveFinder = new Mock<IValidMoveFinder>();
            mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(context))
                .Returns(new int[0]);
            var statusExaminer = new GameStatusExaminer(mockValidMoveFinder.Object);

            //Act
            var result = statusExaminer.DetermineGameStatus(context);

            //Assert
            Assert.Equal(expectedStatus, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldSetCorrectStatusWhenMovesRemainForWhiteButNotBlack(bool isBlackToPlay)
        {
            //Arrange
            var context = new GameContext();
            if (!isBlackToPlay)
            {
                // indicate that it's white to play
                context.SetMovePlayed(); 
            }
            var mockValidMoveFinder = new Mock<IValidMoveFinder>();
            mockValidMoveFinder.Setup(vmf =>
                vmf.IsAnyMoveValid(It.Is<IGameContext>(c => c.MoveNumber % 2 == 1)))
                .Returns(false); // indicate no valid moves remain if black
            mockValidMoveFinder.Setup(vmf =>
                vmf.IsAnyMoveValid(It.Is<IGameContext>(c => c.MoveNumber % 2 == 0)))
                .Returns(true); // indicate some valid moves remain if white
            var statusExaminer = new GameStatusExaminer(mockValidMoveFinder.Object);

            //Act
            var result = statusExaminer.DetermineGameStatus(context);

            //Assert
            Assert.Equal(GameStatus.InProgress, result);
        }

    }
}

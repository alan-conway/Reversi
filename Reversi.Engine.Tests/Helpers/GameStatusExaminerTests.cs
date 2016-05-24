using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Helpers;
using Reversi.Engine.Core;
using Reversi.Engine.Tests.Extensions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Reversi.Engine.Tests.Helpers
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
            var fixture = new Fixture();
            int[] blackSquares = Enumerable.Range(0, numBlackPieces-1).ToArray();
            int[] whiteSquares = Enumerable.Range(numBlackPieces, 63-numBlackPieces).ToArray();
            var context = new GameContext()
                .SetPiece(Piece.Black, blackSquares)
                .SetPiece(Piece.White, whiteSquares);
            fixture.Inject<IGameContext>(context);

            var mockValidMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var context = new GameContext();
            if (!isBlackToPlay)
            {
                // indicate that it's white to play
                context.SetMovePlayed(); 
            }

            var mockValidMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockValidMoveFinder.Setup(vmf =>
                vmf.IsAnyMoveValid(It.Is<IGameContext>(c => c.CurrentPiece == Piece.Black)))
                .Returns(false); // indicate no valid moves remain if black
            mockValidMoveFinder.Setup(vmf =>
                vmf.IsAnyMoveValid(It.Is<IGameContext>(c => c.CurrentPiece == Piece.White)))
                .Returns(true); // indicate some valid moves remain if white
            var statusExaminer = fixture.Create<GameStatusExaminer>();

            //Act
            var result = statusExaminer.DetermineGameStatus(context);

            //Assert
            Assert.Equal(GameStatus.InProgress, result);
        }

    }
}

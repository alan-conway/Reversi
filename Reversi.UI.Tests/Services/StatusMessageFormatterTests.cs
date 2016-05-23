using Moq;
using Reversi.Engine.Core;
using Reversi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.UI.Tests.Services
{
    public class StatusMessageFormatterTests
    {
        [Theory]
        [InlineData(GameStatus.NewGame, 2, 2,  "")]
        [InlineData(GameStatus.InProgress, 31, 31,  "Black: 31  White: 31")]
        [InlineData(GameStatus.Draw, 31, 31,  "Game is a draw  (Black: 31  White: 31)")]
        [InlineData(GameStatus.BlackWins, 59, 5,  "Black wins  (Black: 59  White: 5)")]
        [InlineData(GameStatus.WhiteWins, 14, 50,  "White wins  (Black: 14  White: 50)")]
        public void ShouldDisplayCorrectStatusMessage(GameStatus status,
            int numBlackCells, int numWhiteCells, string expectedMessage)
        {
            //Arrange
            var mockScoreCalc = new Mock<IScoreCalculator>();
            mockScoreCalc.Setup(sc => sc.CalculateScoreForPlayer(Piece.Black, status, It.IsAny<Square[]>()))
                .Returns(numBlackCells);
            mockScoreCalc.Setup(sc => sc.CalculateScoreForPlayer(Piece.White, status, It.IsAny<Square[]>()))
                .Returns(numWhiteCells);
            var msgFormatter = new StatusMessageFormatter(mockScoreCalc.Object);
            
            //Act
            var msg = msgFormatter.GetStatusMessage(status, null);

            //Assert
            Assert.Equal(expectedMessage, msg);
        }
    }
}

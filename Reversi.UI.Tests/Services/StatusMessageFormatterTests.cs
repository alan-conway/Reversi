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
        [InlineData(GameStatus.InProgress, 10, 10,  "Black: 10  White: 10")]
        [InlineData(GameStatus.InProgress, 15, 5,  "Black: 15  White: 5")]
        [InlineData(GameStatus.Draw, 10, 10,  "Game is a draw  (Black: 10  White: 10)")]
        [InlineData(GameStatus.BlackWins, 15, 5,  "Black wins  (Black: 59  White: 5)")]
        [InlineData(GameStatus.WhiteWins, 14, 20,  "White wins  (Black: 14  White: 50)")]
        public void ShouldDisplayCorrectStatusMessage(GameStatus status,
            int numBlackCells, int numWhiteCells, string expectedMessage)
        {
            //Arrange
            var scoreCalculator = new ScoreCalculator();
            var msgFormatter = new StatusMessageFormatter(scoreCalculator);

            var squares = Enumerable.Range(0, 64).Select(x => new Square(Piece.None, false)).ToArray();
            for (int i = 0; i < numBlackCells; i++)
            {
                squares[i].Piece = Piece.Black;
            }
            for (int i = 63; i > 63 - numWhiteCells; i--)
            {
                squares[i].Piece = Piece.White;
            }

            //Act
            var msg = msgFormatter.GetStatusMessage(status, squares);

            //Assert
            Assert.Equal(expectedMessage, msg);
        }
    }
}

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
    public class ScoreCalculatorTests
    {
        [Theory]
        [InlineData(GameStatus.NewGame, 2, 2, 2, 2)]
        [InlineData(GameStatus.InProgress, 10, 10, 10, 10)]
        [InlineData(GameStatus.InProgress, 15, 5, 15, 5)]
        [InlineData(GameStatus.Draw, 10, 10, 10, 10)]
        [InlineData(GameStatus.BlackWins, 15, 5, 59, 5)]  // nb, remaining empty squares
        [InlineData(GameStatus.WhiteWins, 14, 20, 14, 50)]// are scored to the winner
        public void ShouldCalculateCorrectScore(GameStatus status,
            int numBlackCells, int numWhiteCells,
            int expectedBlackScore, int expectedWhiteScore)
        {
            //Arrange
            var scoreCalculator = new ScoreCalculator();

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
            int blackScore = scoreCalculator.CalculateScoreForPlayer(Piece.Black, status, squares);
            int whiteScore = scoreCalculator.CalculateScoreForPlayer(Piece.White, status, squares);
            
            //Assert
            Assert.Equal(expectedBlackScore, blackScore);
            Assert.Equal(expectedWhiteScore, whiteScore);
        }
    }
}

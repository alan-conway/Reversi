using Moq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Minimax.Heuristics
{
    public class WinLoseHeuristicTests
    {
        [Theory]
        [InlineData(GameStatus.NewGame, true, 0)]
        [InlineData(GameStatus.Draw, true, 0)]
        [InlineData(GameStatus.BlackWins, true, 1000)]
        [InlineData(GameStatus.WhiteWins, true, -1000)]
        [InlineData(GameStatus.BlackWins, false, -1000)]
        [InlineData(GameStatus.WhiteWins, false, 1000)]
        public void ShouldReturnCorrectScoreForGameStatus(
            GameStatus status, bool isPlayer1, int expectedScore)
        {
            //Arrange
            var mockStatusExaminer = new Mock<IGameStatusExaminer>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(status);

            var winLoseHeuristic = new WinLoseHeuristic(mockStatusExaminer.Object);

            var mockContext = new Mock<IGameContext>().Object;

            var relativePiece = isPlayer1 ? Piece.Black : Piece.White;

            //Act
            int score = winLoseHeuristic.GetScore(mockContext, relativePiece);

            //Assert
            Assert.Equal(expectedScore, score);
        }
    }
}

using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockStatusExaminer = fixture.Freeze<Mock<IGameStatusExaminer>>();
            var mockContext = fixture.Freeze<Mock<IGameContext>>();

            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(status);
            var relativePiece = isPlayer1 ? Piece.Black : Piece.White;
            var winLoseHeuristic = fixture.Create<WinLoseHeuristic>();

            //Act
            int score = winLoseHeuristic.GetScore(mockContext.Object, relativePiece);

            //Assert
            Assert.Equal(expectedScore, score);
        }
    }
}

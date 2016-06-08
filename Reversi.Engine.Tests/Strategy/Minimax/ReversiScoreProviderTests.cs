using Game.Search.Interfaces;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Shared;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Minimax
{
    public class ReversiScoreProviderTests
    {
        [Theory]
        [InlineData(true, Piece.Black)]
        [InlineData(false, Piece.White)]
        public void ShouldCallHeuristicsWithCorrectPiece(bool isPlayer1, Piece expectedPiece)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());                        

            var mockContext = fixture.Freeze<Mock<IGameContext>>();
            var context = mockContext.Object;
            var mockTreeNode = fixture.Freeze<Mock<IReversiTreeNode>>();
            mockTreeNode.Setup(tn => tn.Context).Returns(context);

            var enemyPiece = expectedPiece == Piece.Black ? Piece.White : Piece.Black;

            var mockHeuristics = fixture.CreateMany<Mock<IHeuristic>>(3);
            var heuristics = mockHeuristics.Select(m => m.Object).ToArray();

            var scoreProvider = new ReversiScoreProvider(heuristics[0], heuristics[1], heuristics[2]);

            //Act
            scoreProvider.EvaluateScore(mockTreeNode.Object, isPlayer1);

            //Assert            
            foreach (var mockHeuristic in mockHeuristics)
            {
                mockHeuristic.Verify(h => h.GetScore(context, expectedPiece), Times.Once());
                mockHeuristic.Verify(h => h.GetScore(context, enemyPiece), Times.Never());
            }
        }
        
        [Theory]
        [InlineData(1000, 100, 1, true, 1101)]
        [InlineData(-1000, 0, 0, true, -1000)]
        [InlineData(0, 25, 2, true, 27)]
        [InlineData(0, 25, -2, true, 23)]
        [InlineData(0, 0, 5, true, 5)]
        [InlineData(0, 6, 0, true, 6)]
        [InlineData(1, 2, 3, false, 6)]
        public void ShouldProvideCorrectScoreWhenGameIsInProgress(int winLoseScore,
            int cornerScore, int mobilityScore, bool isPlayer1, int expectedScore)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var context = fixture.Freeze<Mock<IGameContext>>().Object;
            var mockTreeNode = fixture.Freeze<Mock<IReversiTreeNode>>();
            mockTreeNode.Setup(tn => tn.Context).Returns(context);

            Piece currentPlayer = isPlayer1 ? Piece.Black : Piece.White;
            Piece opponentPlayer = (currentPlayer == Piece.Black ? Piece.White : Piece.Black);

            var winLoseHeuristic = fixture.Create<Mock<IHeuristic>>();
            winLoseHeuristic.Setup(wlh => wlh.GetScore(It.IsAny<IGameContext>(), currentPlayer)).Returns(winLoseScore);
            winLoseHeuristic.Setup(wlh => wlh.GetScore(It.IsAny<IGameContext>(), opponentPlayer)).Returns(winLoseScore * -1);

            var cornerHeuristic = fixture.Create<Mock<IHeuristic>>();
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), currentPlayer)).Returns(cornerScore);
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), opponentPlayer)).Returns(cornerScore * -1);

            var mobilityHeuristic = fixture.Create<Mock<IHeuristic>>();
            mobilityHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), currentPlayer)).Returns(mobilityScore);
            mobilityHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), opponentPlayer)).Returns(mobilityScore * -1);

            var scoreProvider = new ReversiScoreProvider(winLoseHeuristic.Object,
                cornerHeuristic.Object, mobilityHeuristic.Object);

            //Act
            int score = scoreProvider.EvaluateScore(mockTreeNode.Object, isPlayer1);
            int altScore = scoreProvider.EvaluateScore(mockTreeNode.Object, !isPlayer1);

            //Assert
            Assert.Equal(expectedScore, score);
            Assert.Equal(expectedScore * -1, altScore);

        }
    }
}

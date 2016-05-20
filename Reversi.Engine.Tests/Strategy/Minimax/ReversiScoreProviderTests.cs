using Game.Search.Interfaces;
using Moq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using Reversi.Engine.Strategy.Minimax.Interfaces;
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
            var context = new Mock<IGameContext>().Object;
            var mockTreeNode = new Mock<IReversiTreeNode>();
            mockTreeNode.Setup(tn => tn.Context).Returns(context);

            var enemyPiece = expectedPiece == Piece.Black ? Piece.White : Piece.Black;

            var winLoseHeuristic = new Mock<IHeuristic>();
            var cornerHeuristic = new Mock<IHeuristic>();
            var mobilityHeuristic = new Mock<IHeuristic>();
            
            var scoreProvider = new ReversiScoreProvider(winLoseHeuristic.Object,
                cornerHeuristic.Object, mobilityHeuristic.Object);

            //Act
            scoreProvider.EvaluateScore(mockTreeNode.Object, isPlayer1);

            //Assert
            winLoseHeuristic.Verify(h => h.GetScore(context, expectedPiece), Times.Once);
            winLoseHeuristic.Verify(h => h.GetScore(context, enemyPiece), Times.Never);
            cornerHeuristic.Verify(h => h.GetScore(context, expectedPiece), Times.Once);
            cornerHeuristic.Verify(h => h.GetScore(context, enemyPiece), Times.Never);
            mobilityHeuristic.Verify(h => h.GetScore(context, expectedPiece), Times.Once);
            mobilityHeuristic.Verify(h => h.GetScore(context, enemyPiece), Times.Never);
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
            var context = new Mock<IGameContext>().Object;
            var mockTreeNode = new Mock<IReversiTreeNode>();
            mockTreeNode.Setup(tn => tn.Context).Returns(context);

            Piece currentPlayer = isPlayer1 ? Piece.Black : Piece.White;
            Piece opponentPlayer = (currentPlayer == Piece.Black ? Piece.White : Piece.Black);

            var winLoseHeuristic = new Mock<IHeuristic>();
            winLoseHeuristic.Setup(wlh => wlh.GetScore(It.IsAny<IGameContext>(), currentPlayer)).Returns(winLoseScore);
            winLoseHeuristic.Setup(wlh => wlh.GetScore(It.IsAny<IGameContext>(), opponentPlayer)).Returns(winLoseScore * -1);

            var cornerHeuristic = new Mock<IHeuristic>();
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), currentPlayer)).Returns(cornerScore);
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), opponentPlayer)).Returns(cornerScore * -1);

            var mobilityHeuristic = new Mock<IHeuristic>();
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

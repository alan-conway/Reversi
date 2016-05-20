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
        [InlineData(GameStatus.NewGame, true, 0)]
        [InlineData(GameStatus.Draw, true, 0)]
        [InlineData(GameStatus.BlackWins, true, 1000)]
        [InlineData(GameStatus.WhiteWins, true, -1000)]
        [InlineData(GameStatus.BlackWins, false, -1000)]
        [InlineData(GameStatus.WhiteWins, false, 1000)]
        public void ShouldProvideCorrectScoreWhenGameIsNotInProgress(
            GameStatus status, bool isPlayer1, int expectedScore)
        {
            //Arrange
            var mockMoveFinder = new Mock<IValidMoveFinder>();
            var mockStatusExaminer = new Mock<IGameStatusExaminer>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(status);

            var cornerHeuristic = new Mock<IHeuristic>();
            var mobilityHeuristic = new Mock<IHeuristic>();
            var treeNodeBuilder = new Mock<IReversiTreeNodeBuilder>().Object;

            var winLoseHeuristic = new WinLoseHeuristic(mockStatusExaminer.Object);

            var scoreProvider = new ReversiScoreProvider(winLoseHeuristic,
                cornerHeuristic.Object, mobilityHeuristic.Object);
            
            var mockContext = new Mock<IGameContext>().Object;
            var reversiTreeNode = new ReversiTreeNode(-1, mockContext, null, treeNodeBuilder);

            //Act
            int score = scoreProvider.EvaluateScore(reversiTreeNode, isPlayer1);

            //Assert
            Assert.Equal(expectedScore, score);
        }

        [Theory]
        [InlineData(25, 2, true, 27)]
        [InlineData(25, -2, true, 23)]
        [InlineData(0, 5, true, 5)]
        [InlineData(6, 0, true, 6)]
        public void ShouldProvideCorrectScoreWhenGameIsInProgress(int cornerScore,
            int mobilityScore, bool isPlayer1, int expectedScore)
        {
            //Arrange
            var winLoseHeuristic = new Mock<IHeuristic>();
            winLoseHeuristic.Setup(wlh => wlh.GetScore(It.IsAny<IGameContext>(), Piece.Black)).Returns(0);

            var cornerHeuristic = new Mock<IHeuristic>();
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), Piece.Black)).Returns(cornerScore);
            cornerHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), Piece.White)).Returns(cornerScore * -1);

            var mobilityHeuristic = new Mock<IHeuristic>();
            mobilityHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), Piece.Black)).Returns(mobilityScore);
            mobilityHeuristic.Setup(ch => ch.GetScore(It.IsAny<IGameContext>(), Piece.White)).Returns(mobilityScore * -1);

            var scoreProvider = new ReversiScoreProvider(winLoseHeuristic.Object,
                cornerHeuristic.Object, mobilityHeuristic.Object);

            var mockContext = new Mock<IGameContext>().Object;
            var treeNodeBuilder = new Mock<IReversiTreeNodeBuilder>().Object;
            var reversiTreeNode = new ReversiTreeNode(-1, mockContext, null, treeNodeBuilder);

            //Act
            int score = scoreProvider.EvaluateScore(reversiTreeNode, isPlayer1);
            int altScore = scoreProvider.EvaluateScore(reversiTreeNode, !isPlayer1);

            //Assert
            Assert.Equal(expectedScore, score);
            Assert.Equal(expectedScore * -1, altScore);

        }
    }
}

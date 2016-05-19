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
    public class MobilityHeuristicTests
    {
        [Theory]
        [InlineData(Piece.Black, 4, 4, 0)]
        [InlineData(Piece.Black, 14, 14, 0)]
        [InlineData(Piece.Black, 4, 1, 3)]
        [InlineData(Piece.Black, 1, 4, -3)]
        public void ShouldCalculateCorrectMobilityScore(Piece relativePiece,
                   int numBlackMoves, int numWhiteMoves, double expectedScore)
        {
            //Arrange
            IGameContext context = new GameContext();
            var mockMoveFinder = new Mock<IValidMoveFinder>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(context, Piece.Black))
                .Returns(new int[numBlackMoves]);
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(context, Piece.White))
                .Returns(new int[numWhiteMoves]);

            var heuristic = new MobilityHeuristic(mockMoveFinder.Object);
            var altPiece = relativePiece == Piece.Black ? Piece.White : Piece.Black;

            //Act
            var score = heuristic.GetScore(context, relativePiece);
            var altScore = heuristic.GetScore(context, altPiece);

            //Assert
            Assert.Equal(expectedScore, score);
            Assert.Equal(expectedScore * -1.0, altScore);
        }
    }
}

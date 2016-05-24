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
    public class CornerHeuristicTests
    {
        [Theory]
        [InlineData(Piece.Black, new[] { 0 }, new[] { 20 }, 25)]
        [InlineData(Piece.Black, new[] { 0 }, new[] { 7 }, 0)]
        [InlineData(Piece.Black, new[] { 20 }, new[] { 21 }, 0)]
        [InlineData(Piece.Black, new[] { 1 }, new[] { 21 }, -8)] 
        [InlineData(Piece.Black, new[] { 0, 1 }, new[] { 21 }, 25)]
        [InlineData(Piece.Black, new[] { 7, 1 }, new[] { 21 }, 17)]
        [InlineData(Piece.Black, new[] { 1, 8 }, new[] { 21 }, -17)]
        [InlineData(Piece.Black, new[] { 1, 8, 9 }, new[] { 21 }, -25)] 
        public void ShouldCalculateCorrectCornerScore(Piece relativePiece,
            int[] blackLocations, int[] whiteLocations, double expectedScore)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var context = fixture.Freeze<GameContext>();
            foreach(int loc in blackLocations)
            {
                context.SetPiece(loc, Piece.Black);
            }
            foreach (int loc in whiteLocations)
            {
                context.SetPiece(loc, Piece.White);
            }
            var heuristic = fixture.Create<CornerHeuristic>();
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

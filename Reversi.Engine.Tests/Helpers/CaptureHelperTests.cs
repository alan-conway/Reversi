using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Engine.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Reversi.Engine.Tests.Extensions;

namespace Reversi.Engine.Tests.Helpers
{
    public class CaptureHelperTests
    {
        [Theory]
        [InlineData(4, new[] { 0 }, new[] { 3, 2, 1 }, new[] { 3, 2, 1 }, new int[] { })]
        [InlineData(4, new[] { 6 }, new[] { 5, 7 }, new[] { 5 }, new[] { 7 })]
        [InlineData(20, new[] { 27, 36 }, new[] { 28, 35 }, new[] { 28 }, new[] { 35 })]
        [InlineData(29, new[] { 27, 36 }, new[] { 28, 35 }, new[] { 28 }, new[] { 35 })]
        [InlineData(34, new[] { 27, 36 }, new[] { 28, 35 }, new[] { 35 }, new[] { 28 })]
        [InlineData(43, new[] { 27, 36 }, new[] { 28, 35 }, new[] { 35 }, new[] { 28 })]
        [InlineData(27, new[] { 9, 11, 13, 25, 29, 41, 43, 45, 31 }, new[] { 0, 18, 20, 26, 28, 34, 36, 61 }, new[] { 18, 20, 26, 28, 34, 36 }, new[] { 0, 61 })]
        public void ShouldCaptureEnemyPieces(int locationPlayed, 
            int[] blackPieces, int[] whitePieces,
            int[] expectCaptured, int[] expectNotCaptured)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var context = fixture.Create<GameContext>();
            context.SetPiece(Piece.Black, blackPieces)
                   .SetPiece(Piece.White, whitePieces);
            var captureHelper = new CaptureHelper(new LocationHelper());

            //Act
            context.SetPiece(locationPlayed, Piece.Black);
            captureHelper.CaptureEnemyPieces(context, locationPlayed);

            //Assert
            Assert.Equal(1+blackPieces.Union(expectCaptured).Distinct().Count(),
                context.Squares.Count(s => s.Piece == Piece.Black));
            Assert.Equal(expectNotCaptured.Length,
                context.Squares.Count(s => s.Piece == Piece.White));

            foreach (int index in expectCaptured)
            {
                Assert.Equal(Piece.Black, context[index].Piece);
            }
            foreach (int index in expectNotCaptured)
            {
                Assert.NotEqual(Piece.Black, context[index].Piece);
            }
        }
    }
}

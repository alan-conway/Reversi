using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Xunit;
using Reversi.Engine.Helpers;
using Reversi.Engine.Core;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture;
using Reversi.Engine.Tests.Extensions;

namespace Reversi.Engine.Tests.Helpers
{
    public class ValidMoveFinderTests
    {
        private IGameContext _context;
        private ValidMoveFinder _validMoveFinder;

        public ValidMoveFinderTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<ILocationHelper>(new LocationHelper());
            _validMoveFinder = fixture.Create<ValidMoveFinder>();
            _context = fixture.Create<GameContext>();
        }


        [Theory]
        [InlineData(0, true, new[] { 2 }, new[] { 1 })] // captures a piece
        [InlineData(18, true, new[] { 0 }, new[] { 9 })] // captures a piece diagonally
        [InlineData(18, false, new[] { 0, 9 }, new[] { 1 })] // does not capture any piece
        [InlineData(0, false, new[] { 0, 2 }, new[] { 1 })] // already a piece in the location played
        [InlineData(0, false, new[] { 3 }, new[] { 4 })] // does not capture any piece
        public void ShouldIdentifyValidMove(int locationPlayed, bool isExpectedValid,
            int[] blackSquares, int[] whiteSquares)
        {
            //Arrange
            _context.SetPiece(Piece.Black, blackSquares)
                    .SetPiece(Piece.White, whiteSquares);

            //Act
            bool result = _validMoveFinder.IsValidMove(_context, locationPlayed);

            //Assert
            Assert.Equal(isExpectedValid, result);
        }        

        [Theory]
        [InlineData(new[] { 20, 29, 34, 43 }, new[] { 27, 36 }, new[] { 28, 35})] 
        public void ShouldIdentifyAllValidMoves(int[] expected, 
            int[] blackSquares, int[] whiteSquares)
        {
            //Arrange
            _context.SetPiece(Piece.Black, blackSquares)
                    .SetPiece(Piece.White, whiteSquares);

            //Act
            var result = _validMoveFinder.FindAllValidMoves(_context);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(expected.Except(result).Any());
        }

        
    }
}

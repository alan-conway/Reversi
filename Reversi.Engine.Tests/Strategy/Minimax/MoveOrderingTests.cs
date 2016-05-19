using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Minimax
{
    public class MoveOrderingTests
    {
        /// <remarks>
        /// 0 and 7 are corners and are considered very good
        /// 1, 6, 8, 9 are adjacent to corners and are considered very 
        ///  bad IF the corner is empty, and considered indifferently otherwise
        /// </remarks>
        [Theory]
        [InlineData(new[] { 1, 2, 3 }, new[] { 2, 3, 1 })]
        [InlineData(new[] { 1, 2, 3, 0 }, new[] { 0, 2, 3, 1 })]
        [InlineData(new[] { 6, 5, 4 }, new[] { 6, 5, 4 })]
        [InlineData(new[] { 8, 9, 10 }, new[] { 10, 8, 9 })]
        public void ShouldPlacePreferableMovesAtFrontOfList(int[] inputMoves, int[] expectedOrdering)
        {
            //Arrange
            IGameContext context = new GameContext();
            context.SetPiece(7, Piece.Black);
            var moveOrdering = new MoveOrdering();

            //Act
            var orderedMoves = moveOrdering.OrderMoves(context, inputMoves).ToArray();

            //Assert
            Assert.Equal(expectedOrdering.Length, orderedMoves.Length);
            for (int i = 0; i < expectedOrdering.Length; i++)
            {
                Assert.Equal(expectedOrdering[i], orderedMoves[i]);
            }
        }
    }
}

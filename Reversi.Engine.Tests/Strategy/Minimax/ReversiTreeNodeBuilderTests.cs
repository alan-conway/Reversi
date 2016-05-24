using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Minimax.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Minimax
{
    public class ReversiTreeNodeBuilderTests
    {
        [Fact]
        public void ShouldBuildTreeNodesWithExpectedMovesInContext()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var context = fixture.Freeze<GameContext>();

            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(context))
                .Returns(new[] { 1, 2, 3});

            var mockMoveOrdering = fixture.Freeze<Mock<IMoveOrdering>>();
            mockMoveOrdering.Setup(mo => mo.OrderMoves(context, It.IsAny<IEnumerable<int>>()))
                .Returns(new[] { 1, 2, 3 });

            var mockEngine = fixture.Freeze<Mock<IGameEngine>>();            
            mockEngine.Setup(e => e.UpdateBoardWithMove(
                It.Is<Move>(m => m.LocationPlayed == 1), It.IsAny<IGameContext>()))
                .Returns(new Response(new Move(1), GetSquares(1)));

            mockEngine.Setup(e => e.UpdateBoardWithMove(
                It.Is<Move>(m => m.LocationPlayed == 2), It.IsAny<IGameContext>()))
                .Returns(new Response(new Move(2), GetSquares(2)));

            mockEngine.Setup(e => e.UpdateBoardWithMove(
                It.Is<Move>(m => m.LocationPlayed == 3), It.IsAny<IGameContext>()))
                .Returns(new Response(new Move(3), GetSquares(3)));

            var treeNodeBuilder = fixture.Create<ReversiTreeNodeBuilder>();

            //Act
            var treeNodes = treeNodeBuilder.CreateNextTreeNodes(context, mockEngine.Object);

            //Assert
            Assert.Equal(3, treeNodes.Count);
            for(int i = 0; i < treeNodes.Count; i++)
            {
                var treeNode = treeNodes[i];
                Assert.Equal(Piece.None, treeNode.Context.Squares[i].Piece); //previous square 
                Assert.Equal(Piece.Black, treeNode.Context.Squares[i + 1].Piece); // square of interest
                Assert.Equal(Piece.None, treeNode.Context.Squares[i + 2].Piece); // following square
            }

        }

        private Square[] GetSquares(int index)
        {
            var squares = Enumerable.Range(0, 64)
                .Select(x => new Square())
                .ToArray();
            squares[index].Piece = Piece.Black;
            return squares;
        }
    }
}

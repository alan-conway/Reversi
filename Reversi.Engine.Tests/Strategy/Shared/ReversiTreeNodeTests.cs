using Moq;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Strategy.Shared
{
    public class ReversiTreeNodeTests
    {
        [Fact]
        public void ShouldHaveIncrementalDepthAsChildrenOfANode()
        {
            //Arrange
            var mockChildTreeNode = new Mock<IReversiTreeNode>();

            var mockTreeNodeBuilder = new Mock<IReversiTreeNodeBuilder>();
            mockTreeNodeBuilder.Setup(
                tnb => tnb.CreateNextTreeNodes(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()))
                .Returns(new List<IReversiTreeNode>() {
                    mockChildTreeNode.Object        
                });

            var root = new ReversiTreeNode(-1, null, null, mockTreeNodeBuilder.Object);

            //Act
            var children = root.GetChildren();

            //Assert
            Assert.Equal(0, root.Depth);
            mockChildTreeNode.VerifySet(node => node.Depth = 1, Times.Once());
            Assert.Equal(mockChildTreeNode.Object, root.GetChildren().First());
        }
    }
}

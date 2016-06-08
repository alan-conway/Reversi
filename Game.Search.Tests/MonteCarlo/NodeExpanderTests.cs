using Game.Search.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.MonteCarlo
{
    public class NodeExpanderTests
    {
        [Theory]
        [InlineData(3)]
        public void ShouldExpandNode(int childrenToAdd)
        {
            //Arrange
            var root = TestTreeNode.CreateNode(100);
            for(int i = 0; i < childrenToAdd; i++)
            {
                var child = TestTreeNode.CreateNode(i);
                root.AddChild(child);
            }
            var wrappedRoot = new WrappedNode(root);
            var initialNumChildren = wrappedRoot.Children.Count();

            var expander = new NodeExpander();

            //Act
            expander.ExpandRandomNode(wrappedRoot);

            //Assert
            Assert.Equal(initialNumChildren + 1, wrappedRoot.Children.Count());
        }
    }
}

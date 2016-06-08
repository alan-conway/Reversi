using Game.Search.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.MonteCarlo
{
    public class SelectionTests
    {
        [Theory]
        [InlineData(true, 2)]
        [InlineData(false, 3)]
        public void ShouldSelectCorrectImmediateChild(bool isBlackToPlay, int childId)
        {
            //Arrange
            var root = new WrappedNode(TestTreeNode.CreateNode(0), isBlackToPlay);
            var c1 = new WrappedNode(TestTreeNode.CreateNode(1));
            var c2 = new WrappedNode(TestTreeNode.CreateNode(2));
            var c3 = new WrappedNode(TestTreeNode.CreateNode(3));
            root.SetUnexpandedChildren(new[] { c1, c2, c3 });
            for(int i = 0; i < 3; i++)
            {
                root.ExpandChild(0);
            }
            c1.PropagateResults(1);
            c1.PropagateResults(-1);
            c2.PropagateResults(1);
            c3.PropagateResults(-1);
            c3.PropagateResults(-1);

            var selector = new Selector();

            //Act
            var node = selector.SelectNode(root);

            //Assert
            Assert.Equal(childId, ((TestTreeNode)node.OriginalTreeNode).Id);
            
        }


        
    
    }
}

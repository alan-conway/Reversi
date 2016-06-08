using Game.Search.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.MonteCarlo
{
    public class WrappedNodeTests
    {
        [Theory]
        [InlineData(-1, 1, 0, 1)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(1, 1, 1, 0)]
        public void ShouldUpdateResultsCorrectly(int score,
            int numGames, int blackWins, int whiteWins)
        {
            //Arrange 
            var wrappedNode = new WrappedNode(TestTreeNode.CreateNode(1));

            //Act
            wrappedNode.PropagateResults(score);

            //Assert
            Assert.Equal(numGames, wrappedNode.NumCompletedGames);
            Assert.Equal(blackWins, wrappedNode.NumBlackWins);
            Assert.Equal(whiteWins, wrappedNode.NumWhiteWins);
        }

        [Theory]
        [InlineData(1, 2, 3, 1.548)]
        [InlineData(2, 4, 6, 1.447)]
        [InlineData(0, 2, 4, 1.177)]
        public void ShouldCalculateUCTCorrectly(int wins, int played, int parentPlayed, double uct)
        {
            //Arrange 
            var parent = new WrappedNode(TestTreeNode.CreateNode(0));
            for(int i = 0; i < parentPlayed - played; i++)
            {
                parent.PropagateResults(1);
            }
            var node = new WrappedNode(TestTreeNode.CreateNode(1));
            parent.SetUnexpandedChildren(new[] { node });
            parent.ExpandChild(0);

            //Act
            for (int i = 0; i < played; i++)
            {
                node.PropagateResults((i < wins) ? 1 : 0);
            }

            //Assert
            Assert.Equal(uct, Math.Round(node.UCTBlack, 3));
        }
    }
}

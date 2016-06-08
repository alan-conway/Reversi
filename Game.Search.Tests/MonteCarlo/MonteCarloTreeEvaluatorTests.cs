using Game.Search.Interfaces;
using Game.Search.MonteCarlo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.MonteCarlo
{
    public class MonteCarloTreeEvaluatorTests
    {
        [Fact]
        public void ShouldEvaluateTreeCorrectly()
        {
            //Arrange
            TestTreeNode root = CreateTree();
            var simulator = new Mock<ISimulator>();
            simulator.Setup(s => s.SimulateGameToCompletionFromNode(It.IsAny<ITreeNode>()))
                .Returns(-1);
            simulator.Setup(s => s.SimulateGameToCompletionFromNode(It.Is<ITreeNode>(n => (n as TestTreeNode).Id == 31)))
                .Returns(1);
            var expectedResult = 3;

            var evaluator = new MonteCarloTreeEvaluator(new Selector(), new NodeExpander(), simulator.Object);

            //Act
            var result = evaluator.EvaluateTree(root, true, 0.1);

            //Assert
            Assert.Equal(expectedResult, 
                ((TestTreeNode)result.TreeNode).Id);
        }

        private static TestTreeNode CreateTree()
        {
            var root = TestTreeNode.CreateNode(0);
            var c1 = TestTreeNode.CreateNode(1);
            var c2 = TestTreeNode.CreateNode(2);
            var c3 = TestTreeNode.CreateNode(3);
            root.AddChild(c1);
            root.AddChild(c2);
            root.AddChild(c3);
            c1.AddChild(TestTreeNode.CreateNode(11));
            c1.AddChild(TestTreeNode.CreateNode(12));
            c2.AddChild(TestTreeNode.CreateNode(21));
            c3.AddChild(TestTreeNode.CreateNode(31));
            c3.AddChild(TestTreeNode.CreateNode(32));
            return root;
        }

    }

    
}


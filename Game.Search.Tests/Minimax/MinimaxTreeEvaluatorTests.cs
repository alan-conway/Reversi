using Game.Search.Interfaces;
using Game.Search.Minimax;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.Minimax
{
    public class MinimaxTreeEvaluatorTests
    {


        public class TestTreeNode : ITreeNode
        {
            private List<TestTreeNode> _children;

            public TestTreeNode(int id)
            {
                Id = id;
                _children = new List<TestTreeNode>();
            }

            public int Id { get; }
            public int Depth { get; private set; }

            public IEnumerable<ITreeNode> GetChildren()
            {
                return _children; 
            }

            public void AddChild(TestTreeNode child)
            {
                _children.Add(child);
                child.Depth = Depth + 1;
            }
        }
        

        [Fact]
        public void ShouldPerformCorrectMinimaxEvaluationOfSmallTree()
        {
            //Arrange
            TestTreeNode tree = BuildPredefinedTreeOfDepth2();

            Func<ITreeNode, bool> isLeafNode = (t) => !t.GetChildren().Any();

            var mockScoreProvider = new Mock<IScoreProvider>();

            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 1), It.IsAny<bool>()))                
                .Returns(5);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 2), It.IsAny<bool>()))
                .Returns(7);

            var evaluator = new MinimaxTreeEvaluator();

            //Act
            var treeValue = evaluator.EvaluateTree(tree, isLeafNode, 
                mockScoreProvider.Object, true);

            //Assert
            Assert.Equal(7, treeValue.Score);
        }

        

        [Fact]
        public void ShouldPerformCorrectMinimaxEvaluationOfSmallTreeUpToSpecifiedDepth()
        {
            //Arrange
            TestTreeNode tree = BuildPredefinedTreeOfDepth2();
            ((TestTreeNode)tree.GetChildren().First()).AddChild(CreateTreeNode(3));

            int maxDepth = 1;
            Func<ITreeNode, bool> isLeafNode = (t) => !t.GetChildren().Any() || 
                ((TestTreeNode)t).Depth >= maxDepth;

            var mockScoreProvider = new Mock<IScoreProvider>();
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 1), It.IsAny<bool>()))
                .Returns(5);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 2), It.IsAny<bool>()))
                .Returns(7);
            //add this extra node which would change the result if it's considered:
            mockScoreProvider.Setup(se => se.EvaluateScore
                (It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 3), It.IsAny<bool>()))
                .Returns(8);

            var evaluator = new MinimaxTreeEvaluator();

            //Act
            var treeValue = evaluator.EvaluateTree(tree, isLeafNode,
                mockScoreProvider.Object, true);

            //Assert
            Assert.Equal(7, treeValue.Score);
        }

        [Fact]
        public void ShouldAlphaBetaPruneMinimaxTree()
        {
            //Arrange
            // create a tree following a worked example with known results:
            // http://web.cs.ucla.edu/~rosen/161/notes/alphabeta.html
            TestTreeNode tree = BuildPredefinedTreeOfDepth4();

            Func<ITreeNode, bool> isLeafNode = (t) => !t.GetChildren().Any();

            var mockScoreProvider = new Mock<IScoreProvider>();
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 14), It.IsAny<bool>()))
                .Returns(3);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 15), It.IsAny<bool>()))
                .Returns(17);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 16), It.IsAny<bool>()))
                .Returns(2);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 18), It.IsAny<bool>()))
                .Returns(15);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 21), It.IsAny<bool>()))
                .Returns(2);
            mockScoreProvider.Setup(se => se.EvaluateScore(
                It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == 23), It.IsAny<bool>()))
                .Returns(3);
            
            var evaluator = new MinimaxTreeEvaluator();

            int expectedRootValue = 3;
            int[] expectedEvaluationNodeIds = new[] { 14, 15, 16, 18, 21, 23 };

            //Act
            var treeValue = evaluator.EvaluateTree(tree, isLeafNode, 
                mockScoreProvider.Object, true);

            //Assert
            Assert.Equal(expectedRootValue, treeValue.Score);

            mockScoreProvider.Verify(
                se => se.EvaluateScore(It.IsAny<ITreeNode>(), true),
                Times.Exactly(expectedEvaluationNodeIds.Length));

            foreach (int nodeId in expectedEvaluationNodeIds)
            {
                mockScoreProvider.Verify(
                    se => se.EvaluateScore(It.Is<ITreeNode>(t => ((TestTreeNode)t).Id == nodeId), true),
                    Times.Once);
            }
        }


        private TestTreeNode BuildPredefinedTreeOfDepth2()
        {
            TestTreeNode root = CreateTreeNode(0);
            root.AddChild(CreateTreeNode(1));
            root.AddChild(CreateTreeNode(2));
            return root;
        }

        private TestTreeNode BuildPredefinedTreeOfDepth4()
        {
            http://web.cs.ucla.edu/~rosen/161/notes/alphabeta.html
            TestTreeNode root = CreateTreeNode(0);
            var n1 = CreateTreeNode(1);
            var n2 = CreateTreeNode(2);
            root.AddChild(n1);
            root.AddChild(n2);

            //depth 2                    
            var n11 = CreateTreeNode(3);
            var n12 = CreateTreeNode(4);
            n1.AddChild(n11);
            n1.AddChild(n12);

            var n21 = CreateTreeNode(5);
            var n22 = CreateTreeNode(6);
            n2.AddChild(n21);
            n2.AddChild(n22);

            //depth 3
            var n111 = CreateTreeNode(7);
            var n112 = CreateTreeNode(8);
            n11.AddChild(n111);
            n11.AddChild(n112);

            var n121 = CreateTreeNode(9);
            var n122 = CreateTreeNode(10);
            n12.AddChild(n121);
            n12.AddChild(n122);

            var n211 = CreateTreeNode(11);
            var n212 = CreateTreeNode(12);
            n21.AddChild(n211);
            n21.AddChild(n212);

            var n221 = CreateTreeNode(13);
            n22.AddChild(n221);

            //depth 4
            var n1111 = CreateTreeNode(14);
            var n1112 = CreateTreeNode(15);
            n111.AddChild(n1111);
            n111.AddChild(n1112);

            var n1121 = CreateTreeNode(16);
            var n1122 = CreateTreeNode(17);
            n112.AddChild(n1121);
            n112.AddChild(n1122);

            var n1212 = CreateTreeNode(18);
            n121.AddChild(n1212);

            var n1221 = CreateTreeNode(19);
            var n1222 = CreateTreeNode(20);
            n122.AddChild(n1221);
            n122.AddChild(n1222);

            var n2111 = CreateTreeNode(21);
            var n2112 = CreateTreeNode(22);
            n211.AddChild(n2111);
            n211.AddChild(n2112);

            var n2121 = CreateTreeNode(23);
            n212.AddChild(n2121);

            var n2211 = CreateTreeNode(24);
            var n2212 = CreateTreeNode(25);
            n221.AddChild(n2211);
            n221.AddChild(n2212);

            return root;
        }

        private TestTreeNode CreateTreeNode(int id)
        {
            return new TestTreeNode(id);
        }
    }
}

using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.Minimax
{
    /// <summary>
    /// Runs a general minimax algorithm over a game tree with alpha-beta pruning
    /// </summary>
    public class MinimaxTreeEvaluator : IMinimaxTreeEvaluator
    {
        public MinimaxResult EvaluateTree(ITreeNode rootNode, Func<ITreeNode, bool> isLeafNode,
            IScoreProvider scoreProvider, bool isPlayer1)
        {
            var wrappedRoot = new WrappedNode(rootNode);
            CalcScoresForNode(wrappedRoot, isLeafNode, scoreProvider, isPlayer1);

            return new MinimaxResult(
                wrappedRoot.Alpha,
                wrappedRoot.Children.First(c => c.Beta == wrappedRoot.Alpha).OriginalTreeNode);
        }

        /// <summary>
        /// Iterates through the tree.
        /// If the node is a leaf then evaluate the score for that leaf.
        /// Otherwise, propagate the leaf's score up to the alpha or beta of the parent.
        /// If the alpha value surpasses the beta then we can skip evaluating other children.
        /// </summary>
        private void CalcScoresForNode(WrappedNode wrappedNode, 
            Func<ITreeNode, bool> isLeafNode, IScoreProvider scoreProvider,
            bool isPlayer1)
        {
            if (isLeafNode(wrappedNode.OriginalTreeNode))
            {
                var score = scoreProvider.EvaluateScore(wrappedNode.OriginalTreeNode, isPlayer1);
                UpdateNodeWithScore(wrappedNode, score);
                return;
            }

            foreach (var originalChild in wrappedNode.OriginalTreeNode.GetChildren())
            {
                var wrappedChild = new WrappedNode(originalChild);
                wrappedNode.AddChild(wrappedChild);
            
                wrappedChild.Alpha = wrappedNode.Alpha;
                wrappedChild.Beta = wrappedNode.Beta;

                CalcScoresForNode(wrappedChild, isLeafNode, scoreProvider, isPlayer1);

                UpdateNodeWithChildResult(wrappedNode, wrappedChild);

                if (wrappedNode.Alpha >= wrappedNode.Beta)
                {
                    break;
                }
            }
        }

        private void UpdateNodeWithChildResult(WrappedNode node, WrappedNode childNode)
        {
            if (node.NodeType == NodeType.Min)
            {
                node.Beta = Math.Min(node.Beta, childNode.Alpha);
            }
            else
            {
                node.Alpha = Math.Max(node.Alpha, childNode.Beta);
            }
        }

        private void UpdateNodeWithScore(WrappedNode node, double score)
        {
            node.Alpha = node.Beta = score;
        }

    }
}
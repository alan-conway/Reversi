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
                UpdateNodeWithScore(wrappedNode, scoreProvider, isPlayer1);
                return;
            }

            foreach (var originalChild in wrappedNode.OriginalTreeNode.GetChildren())
            {
                bool canPrune = EvaluateChildAndRecurse(wrappedNode, originalChild, isLeafNode, 
                    scoreProvider, isPlayer1);

                if (canPrune)
                {
                    break;
                }
            }
        }

        private bool EvaluateChildAndRecurse(WrappedNode parent, ITreeNode originalChild, Func<ITreeNode, bool> isLeafNode, 
            IScoreProvider scoreProvider, bool isPlayer1)
        {
            var wrappedChild = new WrappedNode(originalChild);
            parent.AddChild(wrappedChild);

            wrappedChild.Alpha = parent.Alpha;
            wrappedChild.Beta = parent.Beta;

            CalcScoresForNode(wrappedChild, isLeafNode, scoreProvider, isPlayer1); // nb recursion

            UpdateNodeWithChildResult(parent, wrappedChild);

            if (parent.Alpha >= parent.Beta)
            {
                return true;
            }
            return false;
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

        private void UpdateNodeWithScore(WrappedNode node, IScoreProvider scoreProvider, bool isPlayer1)
        {
            var score = scoreProvider.EvaluateScore(node.OriginalTreeNode, isPlayer1);
            node.Alpha = node.Beta = score;
        }

    }
}
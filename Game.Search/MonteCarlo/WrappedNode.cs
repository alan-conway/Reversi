using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public class WrappedNode : IUpperConfidenceBoundCalculator
    {
        private List<WrappedNode> _children;
        private List<WrappedNode> _unexpandedChildren;

        public WrappedNode(ITreeNode originalTreeNode):this(originalTreeNode,true)
        {
        }

        public WrappedNode(ITreeNode originalTreeNode, bool isBlackToPlay)
        {
            OriginalTreeNode = originalTreeNode;
            NodeType = isBlackToPlay ? NodeType.BlackToPlay : NodeType.WhiteToPlay;
            _children = new List<WrappedNode>();
            _unexpandedChildren = new List<WrappedNode>();
        }

        public ITreeNode OriginalTreeNode { get; }
        public NodeType NodeType { get; private set; }
        public IEnumerable<WrappedNode> Children { get { return _children; } }

        public IEnumerable<WrappedNode> UnexpandedChildren { get { return _unexpandedChildren; } }
        public WrappedNode Parent { get; private set; }

        public int NumCompletedGames { get; set; }
        public int NumBlackWins { get; set; }
        public int NumWhiteWins { get; set; }

        public double UCTBlack { get { return CalculateUpperConfidenceBound(NumBlackWins); } }
        public double UCTWhite { get { return CalculateUpperConfidenceBound(NumWhiteWins); } }

        public void SetUnexpandedChildren(IEnumerable<WrappedNode> children)
        {
            foreach (var child in children)
            {
                AddUnexpandedChild(child);
            }
        }

        public WrappedNode ExpandChild(int index)
        {
            var child = _unexpandedChildren.ElementAt(index);
            _unexpandedChildren.Remove(child);
            _children.Add(child);
            return child;
        }

        public void PropagateResults(int score)
        {
            NumCompletedGames++;
            if (score > 0)
            {
                NumBlackWins++;
            }
            else if (score < 0)
            {
                NumWhiteWins++;
            }
            if (Parent != null)
            {
                Parent.PropagateResults(score);
            }
        }

        public double CalculateUpperConfidenceBound(int wins, int numGames, int numParentGames, double constant)
        {
            var winRatio = ((double)wins / numGames);
            return winRatio + constant * Math.Sqrt(Math.Log(numParentGames) / numGames);
        }

        private void AddUnexpandedChild(WrappedNode child)
        {
            _unexpandedChildren.Add(child);
            child.Parent = this;
            if (NodeType == NodeType.BlackToPlay)
            {
                child.NodeType = NodeType.WhiteToPlay;
            }
        }

        private double CalculateUpperConfidenceBound(int wins)
        {
            return CalculateUpperConfidenceBound(
                wins, NumCompletedGames, Parent.NumCompletedGames, Math.Sqrt(2));
        }

        
    }
}
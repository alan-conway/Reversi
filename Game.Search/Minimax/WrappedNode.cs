using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.Minimax
{
    /// <summary>
    /// Enrich the ITreeNode supplied by the user to allow each node
    /// to also keep track of its alpha and beta scores, and to know
    /// whether the node is a min-type (i.e. representing the opponents
    /// move) or a max-type (i.e. representing the engine's move)
    /// </summary>
    internal class WrappedNode
    {
        private List<WrappedNode> _children;

        public WrappedNode(ITreeNode originalTreeNode)
        {
            OriginalTreeNode = originalTreeNode;
            NodeType = NodeType.Max;
            Alpha = double.MinValue;
            Beta = double.MaxValue;
            _children = new List<WrappedNode>();
        }

        public ITreeNode OriginalTreeNode { get; }
        public NodeType NodeType { get; private set; }
        public IEnumerable<WrappedNode> Children { get { return _children; } }

        public double Alpha { get; set; }
        public double Beta { get; set; }

        public void AddChild(WrappedNode child)
        {
            _children.Add(child);
            if (NodeType == NodeType.Max)
            {
                child.NodeType = NodeType.Min;
            }
        }
    }
}
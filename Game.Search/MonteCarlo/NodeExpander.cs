using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public class NodeExpander : IExpander
    {
        private Random _random;

        public NodeExpander()
        {
            _random = new Random();
        }

        public WrappedNode ExpandRandomNode(WrappedNode node)
        {
            AddAllUnexpandedChildNodes(node);
            return ExpandRandomChild(node);
        }

        private void AddAllUnexpandedChildNodes(WrappedNode node)
        {
            if (node.UnexpandedChildren.Count() != 0 || node.Children.Count() != 0)
            {
                // already determined child nodes, so do not repeat
                return;
            }

            node.SetUnexpandedChildren(
                node.OriginalTreeNode
                    .GetChildren()
                    .Select(c => new WrappedNode(c)));
        }

        private WrappedNode ExpandRandomChild(WrappedNode node)
        {
            var numUnexpandedChildren = node.UnexpandedChildren.Count();
            if (numUnexpandedChildren > 0)
            {
                var randomIndex = _random.Next(numUnexpandedChildren);
                return node.ExpandChild(randomIndex);
            }
            return node;
        }
    }
}
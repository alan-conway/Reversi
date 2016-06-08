using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public class Selector : ISelector
    {
        public WrappedNode SelectNode(WrappedNode node)
        {
            if (!node.Children.Any() || node.UnexpandedChildren.Any())
            {
                return node;
            }
            else
            {
                var orderedChildren = (node.NodeType == NodeType.BlackToPlay) ?
                    node.Children.OrderByDescending(c => c.UCTBlack) :
                    node.Children.OrderByDescending(c => c.UCTWhite);
                return SelectNode(orderedChildren.First()); //nb recursion
            }
        }
    }
}

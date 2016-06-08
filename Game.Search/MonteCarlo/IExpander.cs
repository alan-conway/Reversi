using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public interface IExpander
    {
        WrappedNode ExpandRandomNode(WrappedNode node);
    }
}

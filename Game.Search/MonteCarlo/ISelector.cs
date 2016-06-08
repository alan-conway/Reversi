using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public interface ISelector
    {
        WrappedNode SelectNode(WrappedNode rootNode);
    }
}

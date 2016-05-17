using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Interfaces
{
    public interface IRandomiser
    {
        int Next(int min, int max);
    }
}

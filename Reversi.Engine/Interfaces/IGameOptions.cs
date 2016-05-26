using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Interfaces
{
    public interface IGameOptions
    {
        bool UserPlaysAsBlack { get; }
        string StrategyName { get; }
        int StrategyLevel { get; }

        IGameOptions Clone();
    }
}

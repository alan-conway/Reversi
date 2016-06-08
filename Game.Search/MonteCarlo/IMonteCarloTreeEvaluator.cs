using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public interface IMonteCarloTreeEvaluator
    {
        MonteCarloResult EvaluateTree(ITreeNode root, bool isPlayer1, double durationSeconds);
    }
}

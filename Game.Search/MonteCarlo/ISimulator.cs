using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public interface ISimulator
    {
        int SimulateGameToCompletionFromNode(ITreeNode node);
    }
}

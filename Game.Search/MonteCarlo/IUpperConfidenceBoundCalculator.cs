using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public interface IUpperConfidenceBoundCalculator
    {
        double CalculateUpperConfidenceBound(int wins, int numGames, int numParentGames, double constant);
    }
}

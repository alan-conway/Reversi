using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Interfaces
{
    /// <summary>
    /// Encapsulates a number of strategies and provides them to 
    /// a caller upon request
    /// </summary>
    public interface IStrategyProvider
    {
        /// <summary>
        /// Provides the named strategy
        /// </summary>
        IMoveStrategy GetStrategy(string name);

        /// <summary>
        /// Returns a summary of all available strategies
        /// </summary>
        IEnumerable<StrategyInfo> GetStrategyInfoCollection();
    }
}

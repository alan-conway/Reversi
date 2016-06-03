using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;

namespace Reversi.Engine.Helpers
{
    /// <summary>
    /// Provides IMoveStrategies
    /// </summary>
    public class StrategyProvider : IStrategyProvider
    {
        private IEnumerable<IMoveStrategy> _strategies;

        public StrategyProvider(IEnumerable<IMoveStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Provides the IMoveStrategy for the name supplied
        /// </summary>
        public IMoveStrategy GetStrategy(string name)
        {
            return _strategies.Single(s => s.StrategyInfo.Name == name);
        }

        /// <summary>
        /// Provides the StrategyInfo for all IMoveStrategies available
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StrategyInfo> GetStrategyInfoCollection()
        {
            return _strategies.Select(s => s.StrategyInfo);
        }
    }
}

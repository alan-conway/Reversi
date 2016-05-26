using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;

namespace Reversi.Engine.Helpers
{
    public class StrategyProvider : IStrategyProvider
    {
        private IEnumerable<IMoveStrategy> _strategies;

        public StrategyProvider(IEnumerable<IMoveStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IMoveStrategy GetStrategy(string name)
        {
            return _strategies.Single(s => s.StrategyInfo.Name == name);
        }

        public IEnumerable<StrategyInfo> GetStrategyInfoCollection()
        {
            return _strategies.Select(s => s.StrategyInfo);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public class StrategyInfo
    {
        public StrategyInfo(string name, bool isMultiLevel, int currentLevel)
        {
            Name = name;
            IsMultiLevel = isMultiLevel;
            CurrentLevel = currentLevel;
        }

        /// <summary>
        /// The name of the strategy
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Indicates whether we can supply a level of strength to use 
        /// when applying this strategy
        /// </summary>
        public bool IsMultiLevel { get; }

        /// <summary>
        /// The current level/strength/depth search of the algorithm
        /// </summary>
        public int CurrentLevel { get; }
    }
}

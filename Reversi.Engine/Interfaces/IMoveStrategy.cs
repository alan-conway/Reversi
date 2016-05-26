using Reversi.Engine.Core;
using System.Collections.Generic;

namespace Reversi.Engine.Interfaces
{
    public interface IMoveStrategy
    {
        /// <summary>
        /// Represents some high-level information about the strategy
        /// </summary>
        StrategyInfo StrategyInfo { get; }

        /// <summary>
        /// Choose a (hopefully optimal) move to play 
        /// </summary>
        Move ChooseMove(IGameContext context, IGameEngine engine);

        /// <summary>
        /// Sets the level/strength/depth that the computer will play at
        /// </summary>
        /// <remarks>
        /// level supplied will be a number 1-10
        /// </remarks>
        void SetLevel(int level);
    }
}
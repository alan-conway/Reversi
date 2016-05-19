using Reversi.Engine.Core;
using System.Collections.Generic;

namespace Reversi.Engine.Interfaces
{
    public interface IMoveStrategy
    {
        /// <summary>
        /// Choose a (hopefully optimal) move to play 
        /// </summary>
        Move ChooseMove(IGameContext context, IGameEngine engine);
    }
}
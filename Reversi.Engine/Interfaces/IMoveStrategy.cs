using System.Collections.Generic;

namespace Reversi.Engine.Interfaces
{
    public interface IMoveStrategy
    {
        /// <summary>
        /// Choose a move to play (hopefully the best one it can, depending on the 
        /// algorithm) 
        /// </summary>
        Move ChooseMove(IGameEngine engine, IGameContext context, IValidMoveFinder validMoveFinder);
    }
}